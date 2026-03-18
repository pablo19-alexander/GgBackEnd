using GoldenGems.Domain.Entities.Chat;
using GoldenGems.Domain.Interfaces;
using GoldenGems.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GoldenGems.Infrastructure.Repositories;

public class ConversationRepository : GenericRepository<Conversation>, IConversationRepository
{
    public ConversationRepository(GoldenGemsDbContext context) : base(context) { }

    public async Task<Conversation?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Conversations.AsNoTracking()
            .Include(c => c.Buyer)
            .Include(c => c.Seller)
            .Include(c => c.Product)
            .Include(c => c.Company)
            .FirstOrDefaultAsync(c => c.Id == id && c.IsActive, cancellationToken);
    }

    public async Task<List<Conversation>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.Conversations.AsNoTracking()
            .Include(c => c.Buyer)
            .Include(c => c.Seller)
            .Include(c => c.Product)
            .Include(c => c.Company)
            .Where(c => c.IsActive && (c.BuyerId == userId || c.SellerId == userId))
            .OrderByDescending(c => c.UpdatedAt ?? c.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<Conversation?> GetActiveByBuyerAndProductAsync(Guid buyerId, Guid productId, CancellationToken cancellationToken = default)
    {
        return await _context.Conversations.AsNoTracking()
            .FirstOrDefaultAsync(c => c.BuyerId == buyerId && c.ProductId == productId
                && c.IsActive && c.Status != ConversationStatus.Closed, cancellationToken);
    }
}
