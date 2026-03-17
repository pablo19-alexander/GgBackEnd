using GoldenGems.Domain.Entities.People;
using GoldenGems.Domain.Interfaces;
using GoldenGems.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GoldenGems.Infrastructure.Repositories;

public class RegionRepository : GenericRepository<Region>, IRegionRepository
{
    public RegionRepository(GoldenGemsDbContext context) : base(context)
    {
    }

    public async Task<List<Region>> GetByDepartmentAsync(string department, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(department)) return new List<Region>();

        var normalizedDepartment = department.Trim().ToUpper();
        return await _context.Regions.AsNoTracking()
            .Where(r => r.Department.ToUpper() == normalizedDepartment && r.IsActive)
            .OrderBy(r => r.MunicipalityName)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<string>> GetDepartmentsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Regions.AsNoTracking()
            .Where(r => r.IsActive)
            .Select(r => r.Department)
            .Distinct()
            .OrderBy(d => d)
            .ToListAsync(cancellationToken);
    }
}
