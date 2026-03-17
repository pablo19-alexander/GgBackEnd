using GoldenGems.Domain.Entities;

namespace GoldenGems.Domain.Interfaces;

/// <summary>
/// Interfaz genérica que define el contrato del patrón Repository.
/// </summary>
public interface IRepository<T> where T : BaseEntity
{
    Task<T> CreateAsync(T entity, CancellationToken cancellationToken = default);
    Task<T> UpdateAsync(T entity, CancellationToken cancellationToken = default);
    Task<List<T>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<List<T>> GetAllActiveAsync(CancellationToken cancellationToken = default);
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<T> DeleteAsync(T entity, CancellationToken cancellationToken = default);
}
