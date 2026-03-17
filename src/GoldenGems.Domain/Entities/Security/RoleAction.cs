namespace GoldenGems.Domain.Entities.Security;

/// <summary>
/// Tabla de relación entre Roles y Acciones.
/// </summary>
public class RoleAction : BaseEntity
{
    public Guid RoleId { get; set; }
    public Role? Role { get; set; }
    public Guid ActionId { get; set; }
    public Actions? Action { get; set; }
}
