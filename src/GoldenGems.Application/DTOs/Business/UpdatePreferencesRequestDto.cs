namespace GoldenGems.Application.DTOs.Business;

public class UpdatePreferencesRequestDto
{
    public Guid? PreferredCompanyId { get; set; }
    public List<Guid> PreferredCategories { get; set; } = new();
    public bool ShowAllCompanies { get; set; } = true;
}
