using GoldenGems.Domain.Entities.Business;

namespace GoldenGems.Domain.Interfaces;

public interface ICompanyRepository : IRepository<Company>
{
    Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<bool> ExistsByNitAsync(string nit, CancellationToken cancellationToken = default);
    Task<Company?> GetByIdWithProductsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Company?> GetDefaultCompanyAsync(CancellationToken cancellationToken = default);
}
