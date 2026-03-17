using GoldenGems.Domain.Entities.People;

namespace GoldenGems.Domain.Interfaces;

/// <summary>
/// Interfaz del repositorio para la entidad Region.
/// </summary>
public interface IRegionRepository : IRepository<Region>
{
    Task<List<Region>> GetByDepartmentAsync(string department, CancellationToken cancellationToken = default);
    Task<List<string>> GetDepartmentsAsync(CancellationToken cancellationToken = default);
}
