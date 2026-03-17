using GoldenGems.Domain.Entities.Security;
using GoldenGems.Domain.Interfaces;
using GoldenGems.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GoldenGems.Infrastructure.Repositories;

public class ActionRepository : GenericRepository<Actions>, IActionRepository
{
    public ActionRepository(GoldenGemsDbContext context) : base(context)
    {
    }

    public override async Task<Actions> CreateAsync(Actions action, CancellationToken cancellationToken = default)
    {
        if (action == null) throw new ArgumentNullException(nameof(action));
        action.Code = action.Code.Trim().ToUpper();
        action.Name = action.Name.Trim();
        return await base.CreateAsync(action, cancellationToken);
    }

    public override async Task<Actions> UpdateAsync(Actions action, CancellationToken cancellationToken = default)
    {
        if (action == null) throw new ArgumentNullException(nameof(action));
        action.Code = action.Code.Trim().ToUpper();
        action.Name = action.Name.Trim();
        return await base.UpdateAsync(action, cancellationToken);
    }

    public override async Task<List<Actions>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.Include(a => a.ActionType).AsNoTracking()
            .OrderBy(a => a.Code).ToListAsync(cancellationToken);
    }

    public override async Task<List<Actions>> GetAllActiveAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.Include(a => a.ActionType).AsNoTracking()
            .Where(a => a.IsActive).OrderBy(a => a.Code).ToListAsync(cancellationToken);
    }

    public override async Task<Actions?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet.Include(a => a.ActionType).AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public async Task<bool> ExistsByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(code)) return false;
        var normalizedCode = code.Trim().ToUpper();
        return await _context.Actions.AsNoTracking()
            .AnyAsync(a => a.Code.ToUpper() == normalizedCode, cancellationToken);
    }

    public async Task<Actions?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(code)) return null;
        var normalizedCode = code.Trim().ToUpper();
        return await _context.Actions.Include(a => a.ActionType).AsNoTracking()
            .FirstOrDefaultAsync(a => a.Code.ToUpper() == normalizedCode, cancellationToken);
    }
}
