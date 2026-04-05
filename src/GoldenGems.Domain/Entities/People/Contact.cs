namespace GoldenGems.Domain.Entities.People;

/// <summary>
/// Entidad Contacto.
/// </summary>
public class Contact : BaseEntity
{
    public string Mobile { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Neighborhood { get; set; } = string.Empty;
    public Guid? MunicipalityId { get; set; }
    public Municipality? Municipality { get; set; }
    public ICollection<Person> People { get; set; } = new List<Person>();
}
