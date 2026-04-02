namespace GoldenGems.Domain.Entities.Payment;

public class Order : BaseEntity
{
    public Guid ConversationId { get; set; }
    public Guid BuyerId { get; set; }
    public Guid SellerId { get; set; }
    public Guid ProductId { get; set; }
    public Guid CompanyId { get; set; }
    public decimal AgreedPrice { get; set; }
    public decimal CommissionPercentage { get; set; }
    public decimal CommissionAmount { get; set; }
    public decimal SellerAmount { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public string? Notes { get; set; }

    // Navigation
    public Chat.Conversation? Conversation { get; set; }
    public Security.User? Buyer { get; set; }
    public Security.User? Seller { get; set; }
    public Business.Product? Product { get; set; }
    public Business.Company? Company { get; set; }
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
}

public enum OrderStatus
{
    Pending,
    Confirmed,
    PaymentPending,
    Paid,
    Shipped,
    Delivered,
    Cancelled,
    Refunded
}
