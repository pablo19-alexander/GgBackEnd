using GoldenGems.Domain.Entities.People;
using GoldenGems.Domain.Interfaces;
using GoldenGems.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GoldenGems.Infrastructure.Repositories;

public class MunicipalityRepository : GenericRepository<Municipality>, IMunicipalityRepository
{
    public MunicipalityRepository(GoldenGemsDbContext context) : base(context)
    {
    }

    public async Task<List<Municipality>> GetByDepartmentIdAsync(Guid departmentId, CancellationToken cancellationToken = default)
    {
        return await _context.Municipalities.AsNoTracking()
            .Include(m => m.Department)
            .Where(m => m.DepartmentId == departmentId && m.IsActive)
            .OrderBy(m => m.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<Municipality?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(code)) return null;
        return await _context.Municipalities.AsNoTracking()
            .Include(m => m.Department)
            .FirstOrDefaultAsync(m => m.Code == code.Trim(), cancellationToken);
    }

    public async Task<List<Municipality>> GetAllActiveWithDepartmentAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Municipalities.AsNoTracking()
            .Include(m => m.Department)
            .Where(m => m.IsActive)
            .OrderBy(m => m.Department.Name).ThenBy(m => m.Name)
            .ToListAsync(cancellationToken);
    }
}
