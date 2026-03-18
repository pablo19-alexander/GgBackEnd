namespace GoldenGems.Application.DTOs.Business;

public class CatalogResponseDto
{
    public List<ProductResponseDto> Items { get; set; } = new();
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
}
