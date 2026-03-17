using GoldenGems.Domain.Entities.Security;

namespace GoldenGems.Domain.Interfaces;

/// <summary>
/// Interfaz del repositorio para la entidad Module.
/// </summary>
public interface IModuleRepository : IRepository<Module>
{
    Task<bool> ExistsByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<Module?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
}
