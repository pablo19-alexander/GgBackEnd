using GoldenGems.Domain.Entities.Payment;
using GoldenGems.Domain.Interfaces;
using GoldenGems.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GoldenGems.Infrastructure.Repositories;

public class CommissionRepository : GenericRepository<Commission>, ICommissionRepository
{
    public CommissionRepository(GoldenGemsDbContext context) : base(context) { }

    public async Task<Commission?> GetByCompanyIdAsync(Guid companyId, CancellationToken cancellationToken = default)
    {
        return await _context.Commissions.AsNoTracking()
            .Include(c => c.Company)
            .FirstOrDefaultAsync(c => c.CompanyId == companyId && c.IsActive, cancellationToken);
    }
}
