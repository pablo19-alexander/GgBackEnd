using GoldenGems.Domain.Entities.Security;

namespace GoldenGems.Domain.Entities.Business;

public class UserPreference : BaseEntity
{
    public Guid UserId { get; set; }
    public Guid? PreferredCompanyId { get; set; }
    public string PreferredCategories { get; set; } = string.Empty; // JSON array of ProductTypeIds
    public bool ShowAllCompanies { get; set; } = true;
    public User? User { get; set; }
    public Company? PreferredCompany { get; set; }
}
