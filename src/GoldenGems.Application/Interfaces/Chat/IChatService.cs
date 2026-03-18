using GoldenGems.Application.Common;
using GoldenGems.Application.DTOs.Chat;

namespace GoldenGems.Application.Interfaces.Chat;

public interface IChatService : IBaseService
{
    Task<ApiResponse<ConversationResponseDto>> StartConversationAsync(Guid buyerId, Guid productId, CancellationToken cancellationToken);
    Task<ApiResponse<List<ConversationResponseDto>>> GetMyConversationsAsync(Guid userId, CancellationToken cancellationToken);
    Task<ApiResponse<List<MessageResponseDto>>> GetMessagesAsync(Guid conversationId, Guid userId, int page, int pageSize, CancellationToken cancellationToken);
    Task<ApiResponse<MessageResponseDto>> SendMessageAsync(Guid conversationId, Guid senderId, string content, CancellationToken cancellationToken);
    Task<ApiResponse<MessageResponseDto>> OfferPriceAsync(Guid conversationId, Guid senderId, decimal price, CancellationToken cancellationToken);
    Task<ApiResponse<ConversationResponseDto>> AcceptPriceAsync(Guid conversationId, Guid userId, CancellationToken cancellationToken);
    Task<ApiResponse<ConversationResponseDto>> RejectPriceAsync(Guid conversationId, Guid userId, CancellationToken cancellationToken);
    Task<ApiResponse<ConversationResponseDto>> CloseConversationAsync(Guid conversationId, Guid userId, CancellationToken cancellationToken);
    Task<ApiResponse<string>> GetWhatsAppLinkAsync(Guid productId, CancellationToken cancellationToken);
}
