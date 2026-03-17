namespace GoldenGems.Domain.Entities.People;

/// <summary>
/// Entidad Región (departamento y municipio).
/// </summary>
public class Region : BaseEntity
{
    public string Department { get; set; } = string.Empty;
    public string MunicipalityCode { get; set; } = string.Empty;
    public string MunicipalityName { get; set; } = string.Empty;
    public ICollection<Contact> Contacts { get; set; } = new List<Contact>();
}
