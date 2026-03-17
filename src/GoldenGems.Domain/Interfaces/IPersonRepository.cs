using GoldenGems.Domain.Entities.People;

namespace GoldenGems.Domain.Interfaces;

/// <summary>
/// Interfaz del repositorio para la entidad Person.
/// </summary>
public interface IPersonRepository : IRepository<Person>
{
    Task<bool> DocumentNumberExistsAsync(string documentNumber, Guid documentTypeId, CancellationToken cancellationToken = default);
    Task<Person?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
}
