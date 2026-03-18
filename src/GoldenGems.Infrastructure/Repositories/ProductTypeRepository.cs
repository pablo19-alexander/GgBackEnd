using GoldenGems.Domain.Entities.Business;
using GoldenGems.Domain.Interfaces;
using GoldenGems.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GoldenGems.Infrastructure.Repositories;

public class ProductTypeRepository : GenericRepository<ProductType>, IProductTypeRepository
{
    public ProductTypeRepository(GoldenGemsDbContext context) : base(context) { }

    public async Task<bool> ExistsByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        var normalized = code.Trim().ToUpper();
        return await _context.ProductTypes.AsNoTracking()
            .AnyAsync(pt => pt.Code.ToUpper() == normalized && pt.IsActive, cancellationToken);
    }
}
