using GoldenGems.Domain.Entities.Business;

namespace GoldenGems.Domain.Interfaces;

public interface IProductTypeRepository : IRepository<ProductType>
{
    Task<bool> ExistsByCodeAsync(string code, CancellationToken cancellationToken = default);
}
