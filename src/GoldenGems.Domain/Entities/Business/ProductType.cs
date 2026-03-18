namespace GoldenGems.Domain.Entities.Business;

public class ProductType : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public ICollection<Product> Products { get; set; } = new List<Product>();
}
