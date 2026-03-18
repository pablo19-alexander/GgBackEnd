using GoldenGems.Domain.Entities.Security;

namespace GoldenGems.Domain.Entities.Chat;

public class Message : BaseEntity
{
    public Guid ConversationId { get; set; }
    public Guid SenderId { get; set; }
    public string Content { get; set; } = string.Empty;
    public MessageType MessageType { get; set; } = MessageType.Text;
    public decimal? OfferedPrice { get; set; }
    public DateTime SentAt { get; set; } = DateTime.UtcNow;
    public bool IsRead { get; set; }
    public Conversation? Conversation { get; set; }
    public User? Sender { get; set; }
}
