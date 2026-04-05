using GoldenGems.Domain.Entities.People;
using GoldenGems.Domain.Interfaces;
using GoldenGems.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GoldenGems.Infrastructure.Repositories;

public class DepartmentRepository : GenericRepository<Department>, IDepartmentRepository
{
    public DepartmentRepository(GoldenGemsDbContext context) : base(context)
    {
    }

    public async Task<List<Department>> GetAllActiveOrderedAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Departments.AsNoTracking()
            .Where(d => d.IsActive)
            .OrderBy(d => d.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<Department?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(code)) return null;
        return await _context.Departments.AsNoTracking()
            .FirstOrDefaultAsync(d => d.Code == code.Trim(), cancellationToken);
    }
}
