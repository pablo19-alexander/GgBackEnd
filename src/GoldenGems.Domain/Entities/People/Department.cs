namespace GoldenGems.Domain.Entities.People;

/// <summary>
/// Departamento (nivel administrativo superior de Colombia). Código DANE de 2 dígitos.
/// </summary>
public class Department : BaseEntity
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public ICollection<Municipality> Municipalities { get; set; } = new List<Municipality>();
}
