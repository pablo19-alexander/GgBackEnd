namespace GoldenGems.Domain.Entities.Payment;

public class Payment : BaseEntity
{
    public Guid OrderId { get; set; }
    public decimal Amount { get; set; }
    public PaymentMethod Method { get; set; }
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
    public string? TransactionId { get; set; }
    public string? GatewayResponse { get; set; }
    public DateTime? PaidAt { get; set; }

    // Navigation
    public Order? Order { get; set; }
}

public enum PaymentMethod
{
    CreditCard,
    DebitCard,
    BankTransfer,
    PSE,
    Nequi,
    Daviplata,
    Cash
}

public enum PaymentStatus
{
    Pending,
    Processing,
    Completed,
    Failed,
    Refunded
}
