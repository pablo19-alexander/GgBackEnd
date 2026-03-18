using GoldenGems.Domain.Entities.Business;

namespace GoldenGems.Domain.Interfaces;

public interface IProductImageRepository : IRepository<ProductImage>
{
    Task<List<ProductImage>> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken = default);
    Task<ProductImage?> GetPrimaryByProductIdAsync(Guid productId, CancellationToken cancellationToken = default);
    Task ClearPrimaryAsync(Guid productId, CancellationToken cancellationToken = default);
}
