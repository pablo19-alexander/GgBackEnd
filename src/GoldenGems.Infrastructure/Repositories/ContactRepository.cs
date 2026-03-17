using GoldenGems.Domain.Entities.People;
using GoldenGems.Domain.Interfaces;
using GoldenGems.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GoldenGems.Infrastructure.Repositories;

public class ContactRepository : GenericRepository<Contact>, IContactRepository
{
    public ContactRepository(GoldenGemsDbContext context) : base(context)
    {
    }

    public async Task<Contact?> GetByPersonIdAsync(Guid personId, CancellationToken cancellationToken = default)
    {
        var person = await _context.People.AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == personId && p.IsActive, cancellationToken);

        if (person?.ContactId == null) return null;

        return await _dbSet.AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == person.ContactId && c.IsActive, cancellationToken);
    }
}
