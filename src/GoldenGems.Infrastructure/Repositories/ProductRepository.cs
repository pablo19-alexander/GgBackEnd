using GoldenGems.Domain.Entities.Business;
using GoldenGems.Domain.Interfaces;
using GoldenGems.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GoldenGems.Infrastructure.Repositories;

public class ProductRepository : GenericRepository<Product>, IProductRepository
{
    public ProductRepository(GoldenGemsDbContext context) : base(context) { }

    public async Task<Product?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Products.AsNoTracking()
            .Include(p => p.Company)
            .Include(p => p.ProductType)
            .Include(p => p.Images.Where(i => i.IsActive).OrderBy(i => i.DisplayOrder))
            .FirstOrDefaultAsync(p => p.Id == id && p.IsActive, cancellationToken);
    }

    public async Task<List<Product>> GetByCompanyIdAsync(Guid companyId, CancellationToken cancellationToken = default)
    {
        return await _context.Products.AsNoTracking()
            .Include(p => p.ProductType)
            .Include(p => p.Images.Where(i => i.IsPrimary && i.IsActive))
            .Where(p => p.CompanyId == companyId && p.IsActive)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Product>> GetAllByCompanyIdAsync(Guid companyId, CancellationToken cancellationToken = default)
    {
        return await _context.Products.AsNoTracking()
            .Include(p => p.ProductType)
            .Include(p => p.Images.Where(i => i.IsPrimary && i.IsActive))
            .Where(p => p.CompanyId == companyId)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<Product?> GetByIdIgnoreActiveAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Products
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<List<Product>> GetByProductTypeIdAsync(Guid productTypeId, CancellationToken cancellationToken = default)
    {
        return await _context.Products.AsNoTracking()
            .Include(p => p.Company)
            .Include(p => p.Images.Where(i => i.IsPrimary && i.IsActive))
            .Where(p => p.ProductTypeId == productTypeId && p.IsActive)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Product>> SearchByNameAsync(string query, CancellationToken cancellationToken = default)
    {
        var normalized = query.Trim().ToUpper();
        return await _context.Products.AsNoTracking()
            .Include(p => p.Company)
            .Include(p => p.ProductType)
            .Include(p => p.Images.Where(i => i.IsPrimary && i.IsActive))
            .Where(p => p.IsActive && (p.Name.ToUpper().Contains(normalized) || p.Description.ToUpper().Contains(normalized)))
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<(List<Product> Items, int TotalCount)> GetCatalogAsync(
        Guid? companyId, Guid? productTypeId, decimal? minPrice, decimal? maxPrice,
        string? search, string? sortBy, int page, int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Products.AsNoTracking()
            .Include(p => p.Company)
            .Include(p => p.ProductType)
            .Include(p => p.Images.Where(i => i.IsPrimary && i.IsActive))
            .Where(p => p.IsActive && p.Company!.IsActive);

        if (companyId.HasValue)
            query = query.Where(p => p.CompanyId == companyId.Value);

        if (productTypeId.HasValue)
            query = query.Where(p => p.ProductTypeId == productTypeId.Value);

        if (minPrice.HasValue)
            query = query.Where(p => p.ReferencePrice >= minPrice.Value);

        if (maxPrice.HasValue)
            query = query.Where(p => p.ReferencePrice <= maxPrice.Value);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var normalized = search.Trim().ToUpper();
            query = query.Where(p => p.Name.ToUpper().Contains(normalized) || p.Description.ToUpper().Contains(normalized));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        query = sortBy?.ToLower() switch
        {
            "price" => query.OrderBy(p => p.ReferencePrice),
            "price_desc" => query.OrderByDescending(p => p.ReferencePrice),
            "name" => query.OrderBy(p => p.Name),
            _ => query.OrderByDescending(p => p.CreatedAt)
        };

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }
}
