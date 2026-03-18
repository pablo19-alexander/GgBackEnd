namespace GoldenGems.Application.DTOs.Business;

public class CompanyResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Logo { get; set; } = string.Empty;
    public string NIT { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string WhatsAppNumber { get; set; } = string.Empty;
    public bool IsDefault { get; set; }
    public Guid OwnerId { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}
