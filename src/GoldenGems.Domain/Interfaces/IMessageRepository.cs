using GoldenGems.Domain.Entities.Chat;

namespace GoldenGems.Domain.Interfaces;

public interface IMessageRepository : IRepository<Message>
{
    Task<List<Message>> GetByConversationIdAsync(Guid conversationId, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<Message?> GetLastPriceOfferAsync(Guid conversationId, CancellationToken cancellationToken = default);
}
