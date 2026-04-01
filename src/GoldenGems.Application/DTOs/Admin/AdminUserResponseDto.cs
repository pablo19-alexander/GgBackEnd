namespace GoldenGems.Application.DTOs.Admin;

public class AdminUserResponseDto
{
    public Guid PersonId { get; set; }
    public Guid UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string? SecondName { get; set; }
    public string FirstLastName { get; set; } = string.Empty;
    public string? SecondLastName { get; set; }
    public Guid DocumentTypeId { get; set; }
    public string? DocumentTypeName { get; set; }
    public string? DocumentNumber { get; set; }
    public string? PhotoUrl { get; set; }
    public List<string> Roles { get; set; } = new();
    public List<Guid> RoleIds { get; set; } = new();
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}
