namespace GoldenGems.Application.DTOs.Chat;

public class ConversationResponseDto
{
    public Guid Id { get; set; }
    public Guid BuyerId { get; set; }
    public string BuyerUsername { get; set; } = string.Empty;
    public Guid SellerId { get; set; }
    public string SellerUsername { get; set; } = string.Empty;
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal ProductReferencePrice { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public decimal? AgreedPrice { get; set; }
    public DateTime CreatedAt { get; set; }
}
