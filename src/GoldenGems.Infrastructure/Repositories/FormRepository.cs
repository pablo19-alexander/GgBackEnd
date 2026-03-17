using GoldenGems.Domain.Entities.Security;
using GoldenGems.Domain.Interfaces;
using GoldenGems.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GoldenGems.Infrastructure.Repositories;

public class FormRepository : GenericRepository<Form>, IFormRepository
{
    public FormRepository(GoldenGemsDbContext context) : base(context)
    {
    }

    public override async Task<Form> CreateAsync(Form form, CancellationToken cancellationToken = default)
    {
        if (form == null) throw new ArgumentNullException(nameof(form));
        form.Code = form.Code.Trim().ToUpper();
        form.Name = form.Name.Trim();
        return await base.CreateAsync(form, cancellationToken);
    }

    public override async Task<Form> UpdateAsync(Form form, CancellationToken cancellationToken = default)
    {
        if (form == null) throw new ArgumentNullException(nameof(form));
        form.Code = form.Code.Trim().ToUpper();
        form.Name = form.Name.Trim();
        return await base.UpdateAsync(form, cancellationToken);
    }

    public override async Task<List<Form>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.AsNoTracking()
            .OrderBy(f => f.ModuleId).ThenBy(f => f.DisplayOrder).ThenBy(f => f.Code)
            .ToListAsync(cancellationToken);
    }

    public override async Task<List<Form>> GetAllActiveAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.AsNoTracking()
            .Where(f => f.IsActive)
            .OrderBy(f => f.ModuleId).ThenBy(f => f.DisplayOrder).ThenBy(f => f.Code)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(code)) return false;
        var normalizedCode = code.Trim().ToUpper();
        return await _context.Forms.AsNoTracking()
            .AnyAsync(f => f.Code.ToUpper() == normalizedCode, cancellationToken);
    }

    public async Task<List<Form>> GetByModuleIdAsync(Guid moduleId, CancellationToken cancellationToken = default)
    {
        return await _context.Forms.AsNoTracking()
            .Where(f => f.ModuleId == moduleId)
            .OrderBy(f => f.DisplayOrder).ThenBy(f => f.Code)
            .ToListAsync(cancellationToken);
    }
}
