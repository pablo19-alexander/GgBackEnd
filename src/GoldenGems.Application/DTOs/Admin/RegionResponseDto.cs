namespace GoldenGems.Application.DTOs.Admin;

public class RegionResponseDto
{
    public Guid Id { get; set; }
    public string Department { get; set; } = string.Empty;
    public string MunicipalityCode { get; set; } = string.Empty;
    public string MunicipalityName { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}
