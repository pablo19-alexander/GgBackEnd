using GoldenGems.Domain.Entities.Security;

namespace GoldenGems.Domain.Interfaces;

/// <summary>
/// Interfaz del repositorio para la entidad User.
/// </summary>
public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);
    Task<User?> GetByIdWithRolesAsync(Guid id, CancellationToken cancellationToken = default);
    Task<User?> GetByIdentifierWithRolesAsync(string identifier, CancellationToken cancellationToken = default);
    Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default);
    Task<bool> UsernameExistsAsync(string username, CancellationToken cancellationToken = default);
    Task AssignRolesAsync(Guid userId, IEnumerable<Guid> roleIds, CancellationToken cancellationToken = default);
    Task<List<User>> GetAllWithDetailsAsync(CancellationToken cancellationToken = default);
    Task<User?> GetByIdTrackingAsync(Guid id, CancellationToken cancellationToken = default);
    Task UpdateRolesAsync(Guid userId, IEnumerable<Guid> roleIds, CancellationToken cancellationToken = default);
}
