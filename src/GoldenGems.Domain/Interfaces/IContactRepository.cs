using GoldenGems.Domain.Entities.People;

namespace GoldenGems.Domain.Interfaces;

/// <summary>
/// Interfaz del repositorio para la entidad Contact.
/// </summary>
public interface IContactRepository : IRepository<Contact>
{
    Task<Contact?> GetByPersonIdAsync(Guid personId, CancellationToken cancellationToken = default);
}
