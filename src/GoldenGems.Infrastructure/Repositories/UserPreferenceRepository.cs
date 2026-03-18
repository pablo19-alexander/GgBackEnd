using GoldenGems.Domain.Entities.Business;
using GoldenGems.Domain.Interfaces;
using GoldenGems.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GoldenGems.Infrastructure.Repositories;

public class UserPreferenceRepository : GenericRepository<UserPreference>, IUserPreferenceRepository
{
    public UserPreferenceRepository(GoldenGemsDbContext context) : base(context) { }

    public async Task<UserPreference?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.UserPreferences.AsNoTracking()
            .Include(up => up.PreferredCompany)
            .FirstOrDefaultAsync(up => up.UserId == userId && up.IsActive, cancellationToken);
    }
}
