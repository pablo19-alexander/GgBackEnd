using GoldenGems.Domain.Entities.People;

namespace GoldenGems.Domain.Entities.Security;

/// <summary>
/// Entidad Usuario del sistema.
/// </summary>
public class User : BaseEntity
{
    public string Email { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public Person? Person { get; set; }
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
