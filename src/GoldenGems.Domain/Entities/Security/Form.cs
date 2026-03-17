namespace GoldenGems.Domain.Entities.Security;

/// <summary>
/// Entidad Formulario dentro de un módulo.
/// </summary>
public class Form : BaseEntity
{
    public string Code { get; set; } = string.Empty;
    public string FormReference { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Route { get; set; }
    public Guid ModuleId { get; set; }
    public Module? Module { get; set; }
    public int DisplayOrder { get; set; }
    public ICollection<Actions> Actions { get; set; } = new List<Actions>();
}
