using GoldenGems.Application.Common;
using GoldenGems.Application.DTOs.Payment;

namespace GoldenGems.Application.Interfaces.Payment;

public interface IPaymentService : IBaseService
{
    Task<ApiResponse<PaymentResponseDto>> InitiatePaymentAsync(Guid userId, InitiatePaymentDto request, CancellationToken cancellationToken);
    Task<ApiResponse<PaymentResponseDto>> ConfirmPaymentAsync(ConfirmPaymentDto request, CancellationToken cancellationToken);
    Task<ApiResponse<List<PaymentResponseDto>>> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken);
}
