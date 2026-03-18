namespace GoldenGems.Domain.Entities.Business;

public class Product : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal ReferencePrice { get; set; }
    public bool IsNegotiable { get; set; }
    public string WhatsAppMessage { get; set; } = string.Empty;
    public Guid CompanyId { get; set; }
    public Guid ProductTypeId { get; set; }
    public Company? Company { get; set; }
    public ProductType? ProductType { get; set; }
    public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();
}
