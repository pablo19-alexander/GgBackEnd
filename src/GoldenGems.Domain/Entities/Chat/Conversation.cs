using GoldenGems.Domain.Entities.Business;
using GoldenGems.Domain.Entities.Security;

namespace GoldenGems.Domain.Entities.Chat;

public class Conversation : BaseEntity
{
    public Guid BuyerId { get; set; }
    public Guid SellerId { get; set; }
    public Guid ProductId { get; set; }
    public Guid CompanyId { get; set; }
    public ConversationStatus Status { get; set; } = ConversationStatus.Open;
    public decimal? AgreedPrice { get; set; }
    public User? Buyer { get; set; }
    public User? Seller { get; set; }
    public Product? Product { get; set; }
    public Company? Company { get; set; }
    public ICollection<Message> Messages { get; set; } = new List<Message>();
}
