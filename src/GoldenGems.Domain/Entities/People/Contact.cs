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
    public Guid? RegionId { get; set; }
    public Region? Region { get; set; }
    public ICollection<Person> People { get; set; } = new List<Person>();
}
