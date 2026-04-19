using GoldenGems.Application.Common;
using GoldenGems.Application.DTOs.Chat;
using GoldenGems.Application.Interfaces.Auth;
using GoldenGems.Application.Interfaces.Chat;
using GoldenGems.Domain.Entities.Chat;
using GoldenGems.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace GoldenGems.Application.Services.Chat;

public class ChatService : BaseService, IChatService
{
    private readonly IConversationRepository _conversationRepository;
    private readonly IMessageRepository _messageRepository;
    private readonly IProductRepository _productRepository;
    private readonly ICompanyRepository _companyRepository;
    private readonly IProfileCompletionService _profileService;

    public ChatService(
        IConversationRepository conversationRepository,
        IMessageRepository messageRepository,
        IProductRepository productRepository,
        ICompanyRepository companyRepository,
        IProfileCompletionService profileService,
        ILogger<ChatService> logger) : base(logger)
    {
        _conversationRepository = conversationRepository;
        _messageRepository = messageRepository;
        _productRepository = productRepository;
        _companyRepository = companyRepository;
        _profileService = profileService;
    }

    public async Task<ApiResponse<ConversationResponseDto>> StartConversationAsync(Guid buyerId, Guid productId, CancellationToken cancellationToken)
    {
        try
        {
            var profileError = await _profileService.ValidateProfileOrError<ConversationResponseDto>(buyerId, cancellationToken);
            if (profileError != null)
                return profileError;

            var product = await _productRepository.GetByIdWithDetailsAsync(productId, cancellationToken);
            if (product == null)
                return ApiResponse<ConversationResponseDto>.ErrorResponse("Producto no encontrado.");

            if (!product.IsNegotiable)
                return ApiResponse<ConversationResponseDto>.ErrorResponse("Este producto no es negociable.");

            var existing = await _conversationRepository.GetActiveByBuyerAndProductAsync(buyerId, productId, cancellationToken);
            if (existing != null)
                return ApiResponse<ConversationResponseDto>.ErrorResponse("Ya tienes una conversación activa para este producto.");

            var conversation = new Conversation
            {
                BuyerId = buyerId,
                SellerId = product.Company!.OwnerId,
                ProductId = productId,
                CompanyId = product.CompanyId,
                Status = ConversationStatus.Open
            };

            var created = await _conversationRepository.CreateAsync(conversation, cancellationToken);

            // Primer mensaje automático: usa InitialChatMessage del producto si existe; si no, genera uno por defecto.
            var initialContent = !string.IsNullOrWhiteSpace(product.InitialChatMessage)
                ? product.InitialChatMessage
                : $"Hola, estoy interesado en \"{product.Name}\". Precio de referencia: ${product.ReferencePrice:N0}. ¿Podemos conversar?";

            await _messageRepository.CreateAsync(new Message
            {
                ConversationId = created.Id,
                SenderId = buyerId,
                Content = initialContent,
                MessageType = MessageType.Text,
                SentAt = DateTime.UtcNow
            }, cancellationToken);

            var detail = await _conversationRepository.GetByIdWithDetailsAsync(created.Id, cancellationToken);
            return ApiResponse<ConversationResponseDto>.SuccessResponse(MapConversation(detail!), "Conversación iniciada.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al iniciar conversación");
            return ApiResponse<ConversationResponseDto>.ErrorResponse("Error al iniciar la conversación.");
        }
    }

    public async Task<ApiResponse<List<ConversationResponseDto>>> GetMyConversationsAsync(Guid userId, CancellationToken cancellationToken)
    {
        try
        {
            var conversations = await _conversationRepository.GetByUserIdAsync(userId, cancellationToken);
            return ApiResponse<List<ConversationResponseDto>>.SuccessResponse(
                conversations.Select(MapConversation).ToList(), $"Se encontraron {conversations.Count} conversaciones");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener conversaciones");
            return ApiResponse<List<ConversationResponseDto>>.ErrorResponse("Error al obtener las conversaciones.");
        }
    }

    public async Task<ApiResponse<List<MessageResponseDto>>> GetMessagesAsync(Guid conversationId, Guid userId, int page, int pageSize, CancellationToken cancellationToken)
    {
        try
        {
            var conversation = await _conversationRepository.GetByIdWithDetailsAsync(conversationId, cancellationToken);
            if (conversation == null)
                return ApiResponse<List<MessageResponseDto>>.ErrorResponse("Conversación no encontrada.");

            if (conversation.BuyerId != userId && conversation.SellerId != userId)
                return ApiResponse<List<MessageResponseDto>>.ErrorResponse("No tienes acceso a esta conversación.");

            var messages = await _messageRepository.GetByConversationIdAsync(conversationId, page, pageSize, cancellationToken);
            return ApiResponse<List<MessageResponseDto>>.SuccessResponse(messages.Select(MapMessage).ToList(), "Mensajes obtenidos.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener mensajes");
            return ApiResponse<List<MessageResponseDto>>.ErrorResponse("Error al obtener los mensajes.");
        }
    }

    public async Task<ApiResponse<MessageResponseDto>> SendMessageAsync(Guid conversationId, Guid senderId, string content, CancellationToken cancellationToken)
    {
        try
        {
            var conversation = await _conversationRepository.GetByIdWithDetailsAsync(conversationId, cancellationToken);
            if (conversation == null)
                return ApiResponse<MessageResponseDto>.ErrorResponse("Conversación no encontrada.");

            if (conversation.BuyerId != senderId && conversation.SellerId != senderId)
                return ApiResponse<MessageResponseDto>.ErrorResponse("No tienes acceso a esta conversación.");

            if (conversation.Status == ConversationStatus.Closed)
                return ApiResponse<MessageResponseDto>.ErrorResponse("La conversación está cerrada.");

            var message = new Message
            {
                ConversationId = conversationId,
                SenderId = senderId,
                Content = content,
                MessageType = MessageType.Text,
                SentAt = DateTime.UtcNow
            };

            var created = await _messageRepository.CreateAsync(message, cancellationToken);
            return ApiResponse<MessageResponseDto>.SuccessResponse(MapMessage(created), "Mensaje enviado.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al enviar mensaje");
            return ApiResponse<MessageResponseDto>.ErrorResponse("Error al enviar el mensaje.");
        }
    }

    public async Task<ApiResponse<MessageResponseDto>> OfferPriceAsync(Guid conversationId, Guid senderId, decimal price, CancellationToken cancellationToken)
    {
        try
        {
            var profileError = await _profileService.ValidateProfileOrError<MessageResponseDto>(senderId, cancellationToken);
            if (profileError != null)
                return profileError;

            var conversation = await _conversationRepository.GetByIdWithDetailsAsync(conversationId, cancellationToken);
            if (conversation == null)
                return ApiResponse<MessageResponseDto>.ErrorResponse("Conversación no encontrada.");

            if (conversation.BuyerId != senderId && conversation.SellerId != senderId)
                return ApiResponse<MessageResponseDto>.ErrorResponse("No tienes acceso a esta conversación.");

            if (conversation.Status == ConversationStatus.Closed || conversation.Status == ConversationStatus.Agreed)
                return ApiResponse<MessageResponseDto>.ErrorResponse("No se puede ofrecer precio en esta conversación.");

            conversation.Status = ConversationStatus.Negotiating;
            await _conversationRepository.UpdateAsync(conversation, cancellationToken);

            var message = new Message
            {
                ConversationId = conversationId,
                SenderId = senderId,
                Content = $"Oferta de precio: ${price:N2}",
                MessageType = MessageType.PriceOffer,
                OfferedPrice = price,
                SentAt = DateTime.UtcNow
            };

            var created = await _messageRepository.CreateAsync(message, cancellationToken);
            return ApiResponse<MessageResponseDto>.SuccessResponse(MapMessage(created), "Oferta enviada.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al ofrecer precio");
            return ApiResponse<MessageResponseDto>.ErrorResponse("Error al enviar la oferta.");
        }
    }

    public async Task<ApiResponse<ConversationResponseDto>> AcceptPriceAsync(Guid conversationId, Guid userId, CancellationToken cancellationToken)
    {
        try
        {
            var profileError = await _profileService.ValidateProfileOrError<ConversationResponseDto>(userId, cancellationToken);
            if (profileError != null)
                return profileError;

            var conversation = await _conversationRepository.GetByIdWithDetailsAsync(conversationId, cancellationToken);
            if (conversation == null)
                return ApiResponse<ConversationResponseDto>.ErrorResponse("Conversación no encontrada.");

            if (conversation.BuyerId != userId && conversation.SellerId != userId)
                return ApiResponse<ConversationResponseDto>.ErrorResponse("No tienes acceso a esta conversación.");

            var lastOffer = await _messageRepository.GetLastPriceOfferAsync(conversationId, cancellationToken);
            if (lastOffer == null)
                return ApiResponse<ConversationResponseDto>.ErrorResponse("No hay oferta de precio pendiente.");

            if (lastOffer.SenderId == userId)
                return ApiResponse<ConversationResponseDto>.ErrorResponse("No puedes aceptar tu propia oferta.");

            conversation.Status = ConversationStatus.Agreed;
            conversation.AgreedPrice = lastOffer.OfferedPrice;
            await _conversationRepository.UpdateAsync(conversation, cancellationToken);

            var acceptMessage = new Message
            {
                ConversationId = conversationId,
                SenderId = userId,
                Content = $"Precio aceptado: ${lastOffer.OfferedPrice:N2}",
                MessageType = MessageType.PriceAccepted,
                OfferedPrice = lastOffer.OfferedPrice,
                SentAt = DateTime.UtcNow
            };
            await _messageRepository.CreateAsync(acceptMessage, cancellationToken);

            var updated = await _conversationRepository.GetByIdWithDetailsAsync(conversationId, cancellationToken);
            return ApiResponse<ConversationResponseDto>.SuccessResponse(MapConversation(updated!), "Precio aceptado.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al aceptar precio");
            return ApiResponse<ConversationResponseDto>.ErrorResponse("Error al aceptar el precio.");
        }
    }

    public async Task<ApiResponse<ConversationResponseDto>> RejectPriceAsync(Guid conversationId, Guid userId, CancellationToken cancellationToken)
    {
        try
        {
            var conversation = await _conversationRepository.GetByIdWithDetailsAsync(conversationId, cancellationToken);
            if (conversation == null)
                return ApiResponse<ConversationResponseDto>.ErrorResponse("Conversación no encontrada.");

            if (conversation.BuyerId != userId && conversation.SellerId != userId)
                return ApiResponse<ConversationResponseDto>.ErrorResponse("No tienes acceso a esta conversación.");

            var rejectMessage = new Message
            {
                ConversationId = conversationId,
                SenderId = userId,
                Content = "Precio rechazado",
                MessageType = MessageType.PriceRejected,
                SentAt = DateTime.UtcNow
            };
            await _messageRepository.CreateAsync(rejectMessage, cancellationToken);

            var updated = await _conversationRepository.GetByIdWithDetailsAsync(conversationId, cancellationToken);
            return ApiResponse<ConversationResponseDto>.SuccessResponse(MapConversation(updated!), "Precio rechazado.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al rechazar precio");
            return ApiResponse<ConversationResponseDto>.ErrorResponse("Error al rechazar el precio.");
        }
    }

    public async Task<ApiResponse<ConversationResponseDto>> CloseConversationAsync(Guid conversationId, Guid userId, CancellationToken cancellationToken)
    {
        try
        {
            var conversation = await _conversationRepository.GetByIdWithDetailsAsync(conversationId, cancellationToken);
            if (conversation == null)
                return ApiResponse<ConversationResponseDto>.ErrorResponse("Conversación no encontrada.");

            if (conversation.BuyerId != userId && conversation.SellerId != userId)
                return ApiResponse<ConversationResponseDto>.ErrorResponse("No tienes acceso a esta conversación.");

            conversation.Status = ConversationStatus.Closed;
            await _conversationRepository.UpdateAsync(conversation, cancellationToken);

            var updated = await _conversationRepository.GetByIdWithDetailsAsync(conversationId, cancellationToken);
            return ApiResponse<ConversationResponseDto>.SuccessResponse(MapConversation(updated!), "Conversación cerrada.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cerrar conversación");
            return ApiResponse<ConversationResponseDto>.ErrorResponse("Error al cerrar la conversación.");
        }
    }

    public async Task<ApiResponse<string>> GetWhatsAppLinkAsync(Guid productId, CancellationToken cancellationToken)
    {
        try
        {
            var product = await _productRepository.GetByIdWithDetailsAsync(productId, cancellationToken);
            if (product == null)
                return ApiResponse<string>.ErrorResponse("Producto no encontrado.");

            var company = product.Company;
            if (company == null || string.IsNullOrWhiteSpace(company.WhatsAppNumber))
                return ApiResponse<string>.ErrorResponse("La empresa no tiene número de WhatsApp configurado.");

            var message = !string.IsNullOrWhiteSpace(product.InitialChatMessage)
                ? product.InitialChatMessage
                : $"Hola, estoy interesado en el producto \"{product.Name}\" de {company.Name}. Precio de referencia: ${product.ReferencePrice:N2}. ¿Podemos negociar?";

            var encodedMessage = Uri.EscapeDataString(message);
            var number = company.WhatsAppNumber.Replace("+", "").Replace(" ", "").Replace("-", "");
            var link = $"https://wa.me/{number}?text={encodedMessage}";

            return ApiResponse<string>.SuccessResponse(link, "Link de WhatsApp generado.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al generar link de WhatsApp");
            return ApiResponse<string>.ErrorResponse("Error al generar el link de WhatsApp.");
        }
    }

    private static ConversationResponseDto MapConversation(Conversation c) => new()
    {
        Id = c.Id, BuyerId = c.BuyerId, BuyerUsername = c.Buyer?.Username ?? string.Empty,
        SellerId = c.SellerId, SellerUsername = c.Seller?.Username ?? string.Empty,
        ProductId = c.ProductId, ProductName = c.Product?.Name ?? string.Empty,
        ProductReferencePrice = c.Product?.ReferencePrice ?? 0,
        CompanyName = c.Company?.Name ?? string.Empty,
        Status = c.Status.ToString(), AgreedPrice = c.AgreedPrice, CreatedAt = c.CreatedAt
    };

    private static MessageResponseDto MapMessage(Message m) => new()
    {
        Id = m.Id, SenderId = m.SenderId, SenderUsername = m.Sender?.Username ?? string.Empty,
        Content = m.Content, MessageType = m.MessageType.ToString(),
        OfferedPrice = m.OfferedPrice, SentAt = m.SentAt, IsRead = m.IsRead
    };
}
