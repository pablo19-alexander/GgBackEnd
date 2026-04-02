using GoldenGems.Application.Common;
using GoldenGems.Application.DTOs.Payment;
using GoldenGems.Application.Interfaces.Payment;
using GoldenGems.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using DomainPayment = GoldenGems.Domain.Entities.Payment;

namespace GoldenGems.Application.Services.Payment;

public class PaymentService : BaseService, IPaymentService
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IOrderRepository _orderRepository;

    public PaymentService(
        IPaymentRepository paymentRepository,
        IOrderRepository orderRepository,
        ILogger<PaymentService> logger) : base(logger)
    {
        _paymentRepository = paymentRepository;
        _orderRepository = orderRepository;
    }

    public async Task<ApiResponse<PaymentResponseDto>> InitiatePaymentAsync(
        Guid userId, InitiatePaymentDto request, CancellationToken cancellationToken)
    {
        try
        {
            var order = await _orderRepository.GetByIdWithDetailsAsync(request.OrderId, cancellationToken);
            if (order == null)
                return ApiResponse<PaymentResponseDto>.ErrorResponse("Orden no encontrada.");

            if (order.BuyerId != userId)
                return ApiResponse<PaymentResponseDto>.ErrorResponse("Solo el comprador puede iniciar el pago.");

            if (order.Status == DomainPayment.OrderStatus.Paid)
                return ApiResponse<PaymentResponseDto>.ErrorResponse("La orden ya fue pagada.");

            if (order.Status == DomainPayment.OrderStatus.Cancelled)
                return ApiResponse<PaymentResponseDto>.ErrorResponse("La orden fue cancelada.");

            if (!Enum.TryParse<DomainPayment.PaymentMethod>(request.Method, true, out var method))
                return ApiResponse<PaymentResponseDto>.ErrorResponse("Método de pago no válido.");

            var payment = new DomainPayment.Payment
            {
                OrderId = order.Id,
                Amount = order.AgreedPrice,
                Method = method,
                Status = DomainPayment.PaymentStatus.Pending,
                TransactionId = $"TXN-{Guid.NewGuid():N}"[..20].ToUpper()
            };

            var created = await _paymentRepository.CreateAsync(payment, cancellationToken);

            // Update order status
            order.Status = DomainPayment.OrderStatus.PaymentPending;
            await _orderRepository.UpdateAsync(order, cancellationToken);

            _logger.LogInformation("Pago iniciado (ID: {Id}) para orden {OrderId}", created.Id, order.Id);
            return ApiResponse<PaymentResponseDto>.SuccessResponse(MapToDto(created), "Pago iniciado. Proceda con la transacción.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al iniciar pago");
            return ApiResponse<PaymentResponseDto>.ErrorResponse("Error al iniciar el pago.");
        }
    }

    public async Task<ApiResponse<PaymentResponseDto>> ConfirmPaymentAsync(
        ConfirmPaymentDto request, CancellationToken cancellationToken)
    {
        try
        {
            var payment = await _paymentRepository.GetByTransactionIdAsync(request.TransactionId, cancellationToken);
            if (payment == null)
                return ApiResponse<PaymentResponseDto>.ErrorResponse("Transacción no encontrada.");

            if (payment.Status == DomainPayment.PaymentStatus.Completed)
                return ApiResponse<PaymentResponseDto>.ErrorResponse("El pago ya fue confirmado.");

            payment.Status = DomainPayment.PaymentStatus.Completed;
            payment.PaidAt = DateTime.UtcNow;
            await _paymentRepository.UpdateAsync(payment, cancellationToken);

            // Update order status to Paid
            var order = await _orderRepository.GetByIdAsync(payment.OrderId, cancellationToken);
            if (order != null)
            {
                order.Status = DomainPayment.OrderStatus.Paid;
                await _orderRepository.UpdateAsync(order, cancellationToken);
            }

            _logger.LogInformation("Pago confirmado (TXN: {TransactionId})", request.TransactionId);
            return ApiResponse<PaymentResponseDto>.SuccessResponse(MapToDto(payment), "Pago confirmado exitosamente.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al confirmar pago");
            return ApiResponse<PaymentResponseDto>.ErrorResponse("Error al confirmar el pago.");
        }
    }

    public async Task<ApiResponse<List<PaymentResponseDto>>> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken)
    {
        try
        {
            var payments = await _paymentRepository.GetByOrderIdAsync(orderId, cancellationToken);
            return ApiResponse<List<PaymentResponseDto>>.SuccessResponse(
                payments.Select(MapToDto).ToList());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener pagos de orden {OrderId}", orderId);
            return ApiResponse<List<PaymentResponseDto>>.ErrorResponse("Error al obtener los pagos.");
        }
    }

    private static PaymentResponseDto MapToDto(DomainPayment.Payment p) => new()
    {
        Id = p.Id,
        OrderId = p.OrderId,
        Amount = p.Amount,
        Method = p.Method.ToString(),
        Status = p.Status.ToString(),
        TransactionId = p.TransactionId,
        PaidAt = p.PaidAt,
        CreatedAt = p.CreatedAt
    };
}
