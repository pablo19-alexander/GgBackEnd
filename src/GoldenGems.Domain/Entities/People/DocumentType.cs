namespace GoldenGems.Domain.Entities.People;

/// <summary>
/// Entidad para tipos de documento (Cédula, Pasaporte, etc.).
/// </summary>
public class DocumentType : BaseEntity
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public ICollection<Person> People { get; set; } = new List<Person>();
}
