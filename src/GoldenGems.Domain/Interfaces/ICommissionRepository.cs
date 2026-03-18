using GoldenGems.Domain.Entities.Payment;

namespace GoldenGems.Domain.Interfaces;

public interface ICommissionRepository : IRepository<Commission>
{
    Task<Commission?> GetByCompanyIdAsync(Guid companyId, CancellationToken cancellationToken = default);
}
