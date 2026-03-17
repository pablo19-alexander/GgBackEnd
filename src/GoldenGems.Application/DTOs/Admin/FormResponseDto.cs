namespace GoldenGems.Application.DTOs.Admin;

public class FormResponseDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string FormReference { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Route { get; set; }
    public Guid ModuleId { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}
