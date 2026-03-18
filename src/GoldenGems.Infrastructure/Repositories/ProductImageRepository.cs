using GoldenGems.Domain.Entities.Business;
using GoldenGems.Domain.Interfaces;
using GoldenGems.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GoldenGems.Infrastructure.Repositories;

public class ProductImageRepository : GenericRepository<ProductImage>, IProductImageRepository
{
    public ProductImageRepository(GoldenGemsDbContext context) : base(context) { }

    public async Task<List<ProductImage>> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        return await _context.ProductImages.AsNoTracking()
            .Where(pi => pi.ProductId == productId && pi.IsActive)
            .OrderBy(pi => pi.DisplayOrder)
            .ToListAsync(cancellationToken);
    }

    public async Task<ProductImage?> GetPrimaryByProductIdAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        return await _context.ProductImages.AsNoTracking()
            .FirstOrDefaultAsync(pi => pi.ProductId == productId && pi.IsPrimary && pi.IsActive, cancellationToken);
    }

    public async Task ClearPrimaryAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        var primaryImages = await _context.ProductImages
            .Where(pi => pi.ProductId == productId && pi.IsPrimary && pi.IsActive)
            .ToListAsync(cancellationToken);

        foreach (var image in primaryImages)
        {
            image.IsPrimary = false;
            image.UpdatedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}
