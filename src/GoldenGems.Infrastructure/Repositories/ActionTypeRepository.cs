using GoldenGems.Domain.Entities.Security;
using GoldenGems.Domain.Interfaces;
using GoldenGems.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GoldenGems.Infrastructure.Repositories;

public class ActionTypeRepository : GenericRepository<ActionType>, IActionTypeRepository
{
    public ActionTypeRepository(GoldenGemsDbContext context) : base(context)
    {
    }

    public async Task<ActionType?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(code)) return null;
        var normalizedCode = code.Trim().ToUpper();
        return await _dbSet.AsNoTracking()
            .FirstOrDefaultAsync(at => at.Code.ToUpper() == normalizedCode, cancellationToken);
    }

    public async Task<bool> ExistsByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(code)) return false;
        var normalizedCode = code.Trim().ToUpper();
        return await _dbSet.AsNoTracking()
            .AnyAsync(at => at.Code.ToUpper() == normalizedCode, cancellationToken);
    }
}
