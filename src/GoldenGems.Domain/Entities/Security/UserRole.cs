namespace GoldenGems.Domain.Entities.Security;

/// <summary>
/// Tabla de relación entre Usuarios y Roles.
/// </summary>
public class UserRole : BaseEntity
{
    public Guid UserId { get; set; }
    public User? User { get; set; }
    public Guid RoleId { get; set; }
    public Role? Role { get; set; }
}
