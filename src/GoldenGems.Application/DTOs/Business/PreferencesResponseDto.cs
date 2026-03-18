namespace GoldenGems.Application.DTOs.Business;

public class PreferencesResponseDto
{
    public Guid Id { get; set; }
    public Guid? PreferredCompanyId { get; set; }
    public string PreferredCompanyName { get; set; } = string.Empty;
    public List<Guid> PreferredCategories { get; set; } = new();
    public bool ShowAllCompanies { get; set; }
}
