namespace GoldenGems.Application.DTOs.Payment;

public class OrderResponseDto
{
    public Guid Id { get; set; }
    public Guid ConversationId { get; set; }
    public Guid BuyerId { get; set; }
    public string BuyerUsername { get; set; } = string.Empty;
    public Guid SellerId { get; set; }
    public string SellerUsername { get; set; } = string.Empty;
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public decimal AgreedPrice { get; set; }
    public decimal CommissionPercentage { get; set; }
    public decimal CommissionAmount { get; set; }
    public decimal SellerAmount { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<PaymentResponseDto> Payments { get; set; } = new();
}

public class CreateOrderFromConversationDto
{
    public Guid ConversationId { get; set; }
    public string? Notes { get; set; }
}
