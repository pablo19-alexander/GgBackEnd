using GoldenGems.Domain.Entities.Security;

namespace GoldenGems.Domain.Interfaces;

/// <summary>
/// Interfaz del repositorio para la entidad Role.
/// </summary>
public interface IRoleRepository : IRepository<Role>
{
    Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<Role?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<List<Guid>> GetExistingRoleIdsAsync(IEnumerable<Guid> roleIds, CancellationToken cancellationToken = default);
}
