using GoldenGems.Domain.Entities.Chat;
using GoldenGems.Domain.Interfaces;
using GoldenGems.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GoldenGems.Infrastructure.Repositories;

public class MessageRepository : GenericRepository<Message>, IMessageRepository
{
    public MessageRepository(GoldenGemsDbContext context) : base(context) { }

    public async Task<List<Message>> GetByConversationIdAsync(Guid conversationId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        return await _context.Messages.AsNoTracking()
            .Include(m => m.Sender)
            .Where(m => m.ConversationId == conversationId && m.IsActive)
            .OrderByDescending(m => m.SentAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<Message?> GetLastPriceOfferAsync(Guid conversationId, CancellationToken cancellationToken = default)
    {
        return await _context.Messages.AsNoTracking()
            .Where(m => m.ConversationId == conversationId && m.MessageType == MessageType.PriceOffer && m.IsActive)
            .OrderByDescending(m => m.SentAt)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
