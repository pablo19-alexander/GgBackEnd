using GoldenGems.Domain.Entities.Business;

namespace GoldenGems.Domain.Interfaces;

public interface IProductRepository : IRepository<Product>
{
    Task<Product?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<Product>> GetByCompanyIdAsync(Guid companyId, CancellationToken cancellationToken = default);
    Task<List<Product>> GetAllByCompanyIdAsync(Guid companyId, CancellationToken cancellationToken = default);
    Task<Product?> GetByIdIgnoreActiveAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<Product>> GetByProductTypeIdAsync(Guid productTypeId, CancellationToken cancellationToken = default);
    Task<List<Product>> SearchByNameAsync(string query, CancellationToken cancellationToken = default);
    Task<(List<Product> Items, int TotalCount)> GetCatalogAsync(
        Guid? companyId, Guid? productTypeId, decimal? minPrice, decimal? maxPrice,
        string? search, string? sortBy, int page, int pageSize,
        CancellationToken cancellationToken = default);
}
