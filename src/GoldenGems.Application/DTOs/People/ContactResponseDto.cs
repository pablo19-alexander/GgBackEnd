namespace GoldenGems.Application.DTOs.People;

public class ContactResponseDto
{
    public Guid Id { get; set; }
    public string Mobile { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? Neighborhood { get; set; }
    public Guid? RegionId { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}
