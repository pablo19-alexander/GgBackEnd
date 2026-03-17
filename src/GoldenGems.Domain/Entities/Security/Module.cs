namespace GoldenGems.Domain.Entities.Security;

/// <summary>
/// Entidad Módulo del sistema.
/// </summary>
public class Module : BaseEntity
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Icon { get; set; }
    public int DisplayOrder { get; set; }
    public ICollection<Form> Forms { get; set; } = new List<Form>();
    public ICollection<Actions> Actions { get; set; } = new List<Actions>();
}
