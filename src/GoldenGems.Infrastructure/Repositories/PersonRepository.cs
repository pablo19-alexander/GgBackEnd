using GoldenGems.Domain.Entities.People;
using GoldenGems.Domain.Interfaces;
using GoldenGems.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GoldenGems.Infrastructure.Repositories;

public class PersonRepository : GenericRepository<Person>, IPersonRepository
{
    public PersonRepository(GoldenGemsDbContext context) : base(context)
    {
    }

    public async Task<bool> DocumentNumberExistsAsync(string documentNumber, Guid documentTypeId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(documentNumber)) return false;
        var normalizedNumber = documentNumber.Trim();
        return await _context.People.AsNoTracking()
            .AnyAsync(p => p.DocumentNumber == normalizedNumber
                        && p.DocumentTypeId == documentTypeId
                        && p.IsActive,
                cancellationToken);
    }

    public async Task<Person?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.People.AsNoTracking()
            .FirstOrDefaultAsync(p => p.UserId == userId && p.IsActive, cancellationToken);
    }
}
