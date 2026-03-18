using GoldenGems.Domain.Entities.Security;

namespace GoldenGems.Domain.Entities.Business;

public class Company : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Logo { get; set; } = string.Empty;
    public string NIT { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string WhatsAppNumber { get; set; } = string.Empty;
    public bool IsDefault { get; set; }
    public Guid OwnerId { get; set; }
    public User? Owner { get; set; }
    public ICollection<Product> Products { get; set; } = new List<Product>();
}
