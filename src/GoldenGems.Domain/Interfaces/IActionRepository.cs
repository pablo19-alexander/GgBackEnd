using GoldenGems.Domain.Entities.Security;

namespace GoldenGems.Domain.Interfaces;

/// <summary>
/// Interfaz del repositorio para la entidad Actions.
/// </summary>
public interface IActionRepository : IRepository<Actions>
{
    Task<bool> ExistsByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<Actions?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
}
