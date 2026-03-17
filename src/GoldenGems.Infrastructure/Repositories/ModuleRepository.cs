using GoldenGems.Domain.Entities.Security;
using GoldenGems.Domain.Interfaces;
using GoldenGems.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GoldenGems.Infrastructure.Repositories;

public class ModuleRepository : GenericRepository<Module>, IModuleRepository
{
    public ModuleRepository(GoldenGemsDbContext context) : base(context)
    {
    }

    public override async Task<Module> CreateAsync(Module module, CancellationToken cancellationToken = default)
    {
        if (module == null) throw new ArgumentNullException(nameof(module));
        module.Code = module.Code.Trim().ToUpper();
        module.Name = module.Name.Trim();
        return await base.CreateAsync(module, cancellationToken);
    }

    public override async Task<Module> UpdateAsync(Module module, CancellationToken cancellationToken = default)
    {
        if (module == null) throw new ArgumentNullException(nameof(module));
        module.Code = module.Code.Trim().ToUpper();
        module.Name = module.Name.Trim();
        return await base.UpdateAsync(module, cancellationToken);
    }

    public override async Task<List<Module>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.AsNoTracking()
            .OrderBy(m => m.DisplayOrder).ThenBy(m => m.Code)
            .ToListAsync(cancellationToken);
    }

    public override async Task<List<Module>> GetAllActiveAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.AsNoTracking()
            .Where(m => m.IsActive)
            .OrderBy(m => m.DisplayOrder).ThenBy(m => m.Code)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(code)) return false;
        var normalizedCode = code.Trim().ToUpper();
        return await _context.Modules.AsNoTracking()
            .AnyAsync(m => m.Code.ToUpper() == normalizedCode, cancellationToken);
    }

    public async Task<Module?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(code)) return null;
        var normalizedCode = code.Trim().ToUpper();
        return await _context.Modules.AsNoTracking()
            .FirstOrDefaultAsync(m => m.Code.ToUpper() == normalizedCode, cancellationToken);
    }
}
