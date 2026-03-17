using GoldenGems.Domain.Entities.Security;
using GoldenGems.Domain.Interfaces;
using GoldenGems.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GoldenGems.Infrastructure.Repositories;

public class RoleRepository : GenericRepository<Role>, IRoleRepository
{
    public RoleRepository(GoldenGemsDbContext context) : base(context)
    {
    }

    public override async Task<Role> CreateAsync(Role role, CancellationToken cancellationToken = default)
    {
        if (role == null) throw new ArgumentNullException(nameof(role));
        role.Name = role.Name.Trim();
        return await base.CreateAsync(role, cancellationToken);
    }

    public override async Task<Role> UpdateAsync(Role role, CancellationToken cancellationToken = default)
    {
        if (role == null) throw new ArgumentNullException(nameof(role));
        role.Name = role.Name.Trim();
        return await base.UpdateAsync(role, cancellationToken);
    }

    public async Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(name)) return false;
        var normalizedName = name.Trim().ToLower();
        return await _context.Roles.AsNoTracking()
            .AnyAsync(r => r.Name.ToLower() == normalizedName, cancellationToken);
    }

    public async Task<Role?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(name)) return null;
        var normalizedName = name.Trim().ToLower();
        return await _context.Roles.AsNoTracking()
            .FirstOrDefaultAsync(r => r.Name.ToLower() == normalizedName, cancellationToken);
    }

    public async Task<List<Guid>> GetExistingRoleIdsAsync(IEnumerable<Guid> roleIds, CancellationToken cancellationToken = default)
    {
        return await _context.Roles.AsNoTracking()
            .Where(r => roleIds.Contains(r.Id))
            .Select(r => r.Id)
            .ToListAsync(cancellationToken);
    }
}
