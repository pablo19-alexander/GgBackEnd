using GoldenGems.Domain.Entities.People;

namespace GoldenGems.Domain.Interfaces;

/// <summary>
/// Interfaz del repositorio para la entidad DocumentType.
/// </summary>
public interface IDocumentTypeRepository : IRepository<DocumentType>
{
    Task<bool> ExistsByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<DocumentType?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
}
