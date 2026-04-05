namespace GoldenGems.Domain.Entities.People;

/// <summary>
/// Municipio de Colombia. Código DANE de 5 dígitos.
/// </summary>
public class Municipality : BaseEntity
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public Guid DepartmentId { get; set; }
    public Department Department { get; set; } = null!;
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public ICollection<Contact> Contacts { get; set; } = new List<Contact>();
}
