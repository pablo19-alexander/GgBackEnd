namespace GoldenGems.Application.DTOs.Payment;

public class PaymentResponseDto
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public decimal Amount { get; set; }
    public string Method { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? TransactionId { get; set; }
    public DateTime? PaidAt { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class InitiatePaymentDto
{
    public Guid OrderId { get; set; }
    public string Method { get; set; } = string.Empty;
}

public class ConfirmPaymentDto
{
    public string TransactionId { get; set; } = string.Empty;
}
