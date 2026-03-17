using GoldenGems.Domain.Entities.People;
using GoldenGems.Domain.Interfaces;
using GoldenGems.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GoldenGems.Infrastructure.Repositories;

public class DocumentTypeRepository : GenericRepository<DocumentType>, IDocumentTypeRepository
{
    public DocumentTypeRepository(GoldenGemsDbContext context) : base(context)
    {
    }

    public async Task<bool> ExistsByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(code)) return false;
        var normalizedCode = code.Trim().ToUpper();
        return await _context.DocumentTypes.AsNoTracking()
            .AnyAsync(dt => dt.Code.ToUpper() == normalizedCode && dt.IsActive, cancellationToken);
    }

    public async Task<DocumentType?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(code)) return null;
        var normalizedCode = code.Trim().ToUpper();
        return await _context.DocumentTypes.AsNoTracking()
            .FirstOrDefaultAsync(dt => dt.Code.ToUpper() == normalizedCode && dt.IsActive, cancellationToken);
    }
}
