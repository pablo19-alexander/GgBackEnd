using GoldenGems.Domain.Entities.Security;
using GoldenGems.Domain.Interfaces;
using GoldenGems.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GoldenGems.Infrastructure.Repositories;

public class UserRepository : GenericRepository<User>, IUserRepository
{
    public UserRepository(GoldenGemsDbContext context) : base(context)
    {
    }

    public override async Task<User> CreateAsync(User user, CancellationToken cancellationToken = default)
    {
        if (user == null) throw new ArgumentNullException(nameof(user));
        user.Email = user.Email.Trim().ToLower();
        user.Username = user.Username.Trim();
        return await base.CreateAsync(user, cancellationToken);
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(email)) return null;
        var normalizedEmail = email.Trim().ToLower();
        return await _dbSet.AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email.ToLower() == normalizedEmail, cancellationToken);
    }

    public async Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(username)) return null;
        return await _dbSet.AsNoTracking()
            .FirstOrDefaultAsync(u => u.Username == username.Trim(), cancellationToken);
    }

    public async Task<User?> GetByIdWithRolesAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty) return null;
        return await _dbSet
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public async Task<User?> GetByIdentifierWithRolesAsync(string identifier, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(identifier)) return null;
        var trimmed = identifier.Trim();
        return await _dbSet
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Email == trimmed || u.Username == trimmed, cancellationToken);
    }

    public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(email)) return false;
        var normalizedEmail = email.Trim().ToLower();
        return await _dbSet.AsNoTracking()
            .AnyAsync(u => u.Email.ToLower() == normalizedEmail, cancellationToken);
    }

    public async Task<bool> UsernameExistsAsync(string username, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(username)) return false;
        return await _dbSet.AsNoTracking()
            .AnyAsync(u => u.Username == username.Trim(), cancellationToken);
    }

    public async Task AssignRolesAsync(Guid userId, IEnumerable<Guid> roleIds, CancellationToken cancellationToken = default)
    {
        foreach (var roleId in roleIds)
        {
            await _context.UserRoles.AddAsync(new UserRole
            {
                UserId = userId,
                RoleId = roleId
            }, cancellationToken);
        }
        await _context.SaveChangesAsync(cancellationToken);
    }
}
