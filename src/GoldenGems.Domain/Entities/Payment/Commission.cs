using GoldenGems.Domain.Entities.Business;

namespace GoldenGems.Domain.Entities.Payment;

public class Commission : BaseEntity
{
    public Guid CompanyId { get; set; }
    public decimal Percentage { get; set; }
    public decimal? MinAmount { get; set; }
    public decimal? MaxAmount { get; set; }
    public Company? Company { get; set; }
}
