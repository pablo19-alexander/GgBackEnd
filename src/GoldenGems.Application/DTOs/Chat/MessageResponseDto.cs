namespace GoldenGems.Application.DTOs.Chat;

public class MessageResponseDto
{
    public Guid Id { get; set; }
    public Guid SenderId { get; set; }
    public string SenderUsername { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string MessageType { get; set; } = string.Empty;
    public decimal? OfferedPrice { get; set; }
    public DateTime SentAt { get; set; }
    public bool IsRead { get; set; }
}
