using GoldenGems.Domain.Entities.Business;
using GoldenGems.Domain.Interfaces;
using GoldenGems.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GoldenGems.Infrastructure.Repositories;

public class CompanyRepository : GenericRepository<Company>, ICompanyRepository
{
    public CompanyRepository(GoldenGemsDbContext context) : base(context) { }

    public async Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        var normalized = name.Trim().ToUpper();
        return await _context.Companies.AsNoTracking()
            .AnyAsync(c => c.Name.ToUpper() == normalized && c.IsActive, cancellationToken);
    }

    public async Task<bool> ExistsByNitAsync(string nit, CancellationToken cancellationToken = default)
    {
        var normalized = nit.Trim();
        return await _context.Companies.AsNoTracking()
            .AnyAsync(c => c.NIT == normalized && c.IsActive, cancellationToken);
    }

    public async Task<Company?> GetByIdWithProductsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Companies.AsNoTracking()
            .Include(c => c.Products.Where(p => p.IsActive))
            .FirstOrDefaultAsync(c => c.Id == id && c.IsActive, cancellationToken);
    }

    public async Task<Company?> GetDefaultCompanyAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Companies.AsNoTracking()
            .FirstOrDefaultAsync(c => c.IsDefault && c.IsActive, cancellationToken);
    }
}
