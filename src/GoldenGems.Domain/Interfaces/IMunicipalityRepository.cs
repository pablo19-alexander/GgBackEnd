using GoldenGems.Domain.Entities.People;

namespace GoldenGems.Domain.Interfaces;

/// <summary>
/// Interfaz del repositorio para la entidad Municipality.
/// </summary>
public interface IMunicipalityRepository : IRepository<Municipality>
{
    Task<List<Municipality>> GetByDepartmentIdAsync(Guid departmentId, CancellationToken cancellationToken = default);
    Task<Municipality?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<List<Municipality>> GetAllActiveWithDepartmentAsync(CancellationToken cancellationToken = default);
}
