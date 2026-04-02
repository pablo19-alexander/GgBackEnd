using GoldenGems.Application.Common;
using GoldenGems.Application.DTOs.Payment;

namespace GoldenGems.Application.Interfaces.Payment;

public interface IOrderService : IBaseService
{
    Task<ApiResponse<OrderResponseDto>> CreateFromConversationAsync(Guid userId, CreateOrderFromConversationDto request, CancellationToken cancellationToken);
    Task<ApiResponse<OrderResponseDto>> GetByIdAsync(Guid orderId, Guid userId, CancellationToken cancellationToken);
    Task<ApiResponse<List<OrderResponseDto>>> GetMyOrdersAsync(Guid userId, CancellationToken cancellationToken);
    Task<ApiResponse<OrderResponseDto>> UpdateStatusAsync(Guid orderId, string status, CancellationToken cancellationToken);
    Task<ApiResponse<OrderResponseDto>> CancelAsync(Guid orderId, Guid userId, CancellationToken cancellationToken);
}
