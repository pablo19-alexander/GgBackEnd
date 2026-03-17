using GoldenGems.Domain.Entities.Security;

namespace GoldenGems.Domain.Interfaces;

/// <summary>
/// Interfaz del repositorio para la entidad ActionType.
/// </summary>
public interface IActionTypeRepository : IRepository<ActionType>
{
    Task<ActionType?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<bool> ExistsByCodeAsync(string code, CancellationToken cancellationToken = default);
}
