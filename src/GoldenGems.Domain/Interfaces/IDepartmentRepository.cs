using GoldenGems.Domain.Entities.People;

namespace GoldenGems.Domain.Interfaces;

/// <summary>
/// Interfaz del repositorio para la entidad Department.
/// </summary>
public interface IDepartmentRepository : IRepository<Department>
{
    Task<List<Department>> GetAllActiveOrderedAsync(CancellationToken cancellationToken = default);
    Task<Department?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
}
