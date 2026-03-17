namespace GoldenGems.Domain.Entities.Security;

/// <summary>
/// Entidad Acción - Define las acciones disponibles en módulos, formularios y procesos.
/// </summary>
public class Actions : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public Guid ActionTypeId { get; set; }
    public ActionType? ActionType { get; set; }
    public Guid? ModuleId { get; set; }
    public Guid? FormId { get; set; }
    public Module? Module { get; set; }
    public Form? Form { get; set; }
    public ICollection<RoleAction> RoleActions { get; set; } = new List<RoleAction>();
}
