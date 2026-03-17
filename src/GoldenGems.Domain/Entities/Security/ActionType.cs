namespace GoldenGems.Domain.Entities.Security;

/// <summary>
/// Catálogo de tipos de acción disponibles en el sistema.
/// </summary>
public class ActionType : BaseEntity
{
    public string Code { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ICollection<Actions> Actions { get; set; } = new List<Actions>();
}
