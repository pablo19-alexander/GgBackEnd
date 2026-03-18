using GoldenGems.Domain.Entities.Business;

namespace GoldenGems.Domain.Interfaces;

public interface IUserPreferenceRepository : IRepository<UserPreference>
{
    Task<UserPreference?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
}
