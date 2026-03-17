using GoldenGems.Domain.Entities.Security;

namespace GoldenGems.Domain.Interfaces;

/// <summary>
/// Interfaz del repositorio para la entidad Form.
/// </summary>
public interface IFormRepository : IRepository<Form>
{
    Task<bool> ExistsByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<List<Form>> GetByModuleIdAsync(Guid moduleId, CancellationToken cancellationToken = default);
}
