using GoldenGems.Application.Common;
using GoldenGems.Application.DTOs.Payment;
using GoldenGems.Application.Interfaces.Payment;
using GoldenGems.Domain.Entities.Payment;
using GoldenGems.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace GoldenGems.Application.Services.Payment;

public class OrderService : BaseService, IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IConversationRepository _conversationRepository;
    private readonly ICommissionRepository _commissionRepository;

    public OrderService(
        IOrderRepository orderRepository,
        IConversationRepository conversationRepository,
        ICommissionRepository commissionRepository,
        ILogger<OrderService> logger) : base(logger)
    {
        _orderRepository = orderRepository;
        _conversationRepository = conversationRepository;
        _commissionRepository = commissionRepository;
    }

    public async Task<ApiResponse<OrderResponseDto>> CreateFromConversationAsync(
        Guid userId, CreateOrderFromConversationDto request, CancellationToken cancellationToken)
    {
        try
        {
            var conversation = await _conversationRepository.GetByIdWithDetailsAsync(request.ConversationId, cancellationToken);
            if (conversation == null)
                return ApiResponse<OrderResponseDto>.ErrorResponse("Conversación no encontrada.");

            if (conversation.Status.ToString() != "Agreed")
                return ApiResponse<OrderResponseDto>.ErrorResponse("La conversación debe estar en estado 'Agreed' para crear una orden.");

            if (conversation.BuyerId != userId && conversation.SellerId != userId)
                return ApiResponse<OrderResponseDto>.ErrorResponse("No tienes acceso a esta conversación.");

            if (!conversation.AgreedPrice.HasValue || conversation.AgreedPrice.Value <= 0)
                return ApiResponse<OrderResponseDto>.ErrorResponse("No hay precio acordado en la conversación.");

            // Check if order already exists for this conversation
            var existingOrder = await _orderRepository.GetByConversationIdAsync(request.ConversationId, cancellationToken);
            if (existingOrder != null)
                return ApiResponse<OrderResponseDto>.ErrorResponse("Ya existe una orden para esta conversación.");

            // Calculate commission
            var commission = await _commissionRepository.GetByCompanyIdAsync(conversation.CompanyId, cancellationToken);
            var commissionPercentage = commission?.Percentage ?? 0;
            var agreedPrice = conversation.AgreedPrice.Value;
            var commissionAmount = Math.Round(agreedPrice * commissionPercentage / 100, 2);
            var sellerAmount = agreedPrice - commissionAmount;

            var order = new Order
            {
                ConversationId = conversation.Id,
                BuyerId = conversation.BuyerId,
                SellerId = conversation.SellerId,
                ProductId = conversation.ProductId,
                CompanyId = conversation.CompanyId,
                AgreedPrice = agreedPrice,
                CommissionPercentage = commissionPercentage,
                CommissionAmount = commissionAmount,
                SellerAmount = sellerAmount,
                Status = OrderStatus.Confirmed,
                Notes = request.Notes
            };

            var created = await _orderRepository.CreateAsync(order, cancellationToken);
            var detail = await _orderRepository.GetByIdWithDetailsAsync(created.Id, cancellationToken);

            _logger.LogInformation("Orden creada exitosamente (ID: {Id}) desde conversación {ConvId}", created.Id, conversation.Id);
            return ApiResponse<OrderResponseDto>.SuccessResponse(MapToDto(detail!), "Orden creada exitosamente.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear orden");
            return ApiResponse<OrderResponseDto>.ErrorResponse("Error al crear la orden.");
        }
    }

    public async Task<ApiResponse<OrderResponseDto>> GetByIdAsync(Guid orderId, Guid userId, CancellationToken cancellationToken)
    {
        try
        {
            var order = await _orderRepository.GetByIdWithDetailsAsync(orderId, cancellationToken);
            if (order == null)
                return ApiResponse<OrderResponseDto>.ErrorResponse("Orden no encontrada.");

            if (order.BuyerId != userId && order.SellerId != userId)
                return ApiResponse<OrderResponseDto>.ErrorResponse("No tienes acceso a esta orden.");

            return ApiResponse<OrderResponseDto>.SuccessResponse(MapToDto(order));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener orden {Id}", orderId);
            return ApiResponse<OrderResponseDto>.ErrorResponse("Error al obtener la orden.");
        }
    }

    public async Task<ApiResponse<List<OrderResponseDto>>> GetMyOrdersAsync(Guid userId, CancellationToken cancellationToken)
    {
        try
        {
            var buyerOrders = await _orderRepository.GetByBuyerIdAsync(userId, cancellationToken);
            var sellerOrders = await _orderRepository.GetBySellerIdAsync(userId, cancellationToken);
            var allOrders = buyerOrders.Concat(sellerOrders)
                .GroupBy(o => o.Id).Select(g => g.First())
                .OrderByDescending(o => o.CreatedAt).ToList();

            return ApiResponse<List<OrderResponseDto>>.SuccessResponse(
                allOrders.Select(MapToDto).ToList(), $"Se encontraron {allOrders.Count} órdenes.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener órdenes del usuario {UserId}", userId);
            return ApiResponse<List<OrderResponseDto>>.ErrorResponse("Error al obtener las órdenes.");
        }
    }

    public async Task<ApiResponse<OrderResponseDto>> UpdateStatusAsync(Guid orderId, string status, CancellationToken cancellationToken)
    {
        try
        {
            var order = await _orderRepository.GetByIdAsync(orderId, cancellationToken);
            if (order == null || !order.IsActive)
                return ApiResponse<OrderResponseDto>.ErrorResponse("Orden no encontrada.");

            if (!Enum.TryParse<OrderStatus>(status, true, out var newStatus))
                return ApiResponse<OrderResponseDto>.ErrorResponse("Estado no válido.");

            order.Status = newStatus;
            await _orderRepository.UpdateAsync(order, cancellationToken);
            var detail = await _orderRepository.GetByIdWithDetailsAsync(orderId, cancellationToken);

            return ApiResponse<OrderResponseDto>.SuccessResponse(MapToDto(detail!), "Estado actualizado.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar estado de orden {Id}", orderId);
            return ApiResponse<OrderResponseDto>.ErrorResponse("Error al actualizar el estado.");
        }
    }

    public async Task<ApiResponse<OrderResponseDto>> CancelAsync(Guid orderId, Guid userId, CancellationToken cancellationToken)
    {
        try
        {
            var order = await _orderRepository.GetByIdAsync(orderId, cancellationToken);
            if (order == null || !order.IsActive)
                return ApiResponse<OrderResponseDto>.ErrorResponse("Orden no encontrada.");

            if (order.BuyerId != userId)
                return ApiResponse<OrderResponseDto>.ErrorResponse("Solo el comprador puede cancelar la orden.");

            if (order.Status == OrderStatus.Paid || order.Status == OrderStatus.Shipped || order.Status == OrderStatus.Delivered)
                return ApiResponse<OrderResponseDto>.ErrorResponse("No se puede cancelar una orden que ya fue pagada o enviada.");

            order.Status = OrderStatus.Cancelled;
            await _orderRepository.UpdateAsync(order, cancellationToken);
            var detail = await _orderRepository.GetByIdWithDetailsAsync(orderId, cancellationToken);

            return ApiResponse<OrderResponseDto>.SuccessResponse(MapToDto(detail!), "Orden cancelada.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cancelar orden {Id}", orderId);
            return ApiResponse<OrderResponseDto>.ErrorResponse("Error al cancelar la orden.");
        }
    }

    private static OrderResponseDto MapToDto(Order o) => new()
    {
        Id = o.Id,
        ConversationId = o.ConversationId,
        BuyerId = o.BuyerId,
        BuyerUsername = o.Buyer?.Username ?? string.Empty,
        SellerId = o.SellerId,
        SellerUsername = o.Seller?.Username ?? string.Empty,
        ProductId = o.ProductId,
        ProductName = o.Product?.Name ?? string.Empty,
        CompanyName = o.Company?.Name ?? string.Empty,
        AgreedPrice = o.AgreedPrice,
        CommissionPercentage = o.CommissionPercentage,
        CommissionAmount = o.CommissionAmount,
        SellerAmount = o.SellerAmount,
        Status = o.Status.ToString(),
        Notes = o.Notes,
        CreatedAt = o.CreatedAt,
        Payments = o.Payments?.Select(p => new PaymentResponseDto
        {
            Id = p.Id,
            OrderId = p.OrderId,
            Amount = p.Amount,
            Method = p.Method.ToString(),
            Status = p.Status.ToString(),
            TransactionId = p.TransactionId,
            PaidAt = p.PaidAt,
            CreatedAt = p.CreatedAt
        }).ToList() ?? new()
    };
}
