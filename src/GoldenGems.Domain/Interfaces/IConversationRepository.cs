using GoldenGems.Domain.Entities.Chat;

namespace GoldenGems.Domain.Interfaces;

public interface IConversationRepository : IRepository<Conversation>
{
    Task<Conversation?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<Conversation>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Conversation?> GetActiveByBuyerAndProductAsync(Guid buyerId, Guid productId, CancellationToken cancellationToken = default);
}
