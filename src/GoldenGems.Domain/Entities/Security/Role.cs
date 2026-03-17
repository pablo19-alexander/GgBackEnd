namespace GoldenGems.Domain.Entities.Security;

/// <summary>
/// Entidad Rol del sistema.
/// </summary>
public class Role : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ICollection<RoleAction> RoleActions { get; set; } = new List<RoleAction>();
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
