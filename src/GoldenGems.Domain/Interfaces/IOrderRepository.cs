using GoldenGems.Domain.Entities.Payment;

namespace GoldenGems.Domain.Interfaces;

public interface IOrderRepository : IRepository<Order>
{
    Task<Order?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<Order>> GetByBuyerIdAsync(Guid buyerId, CancellationToken cancellationToken = default);
    Task<List<Order>> GetBySellerIdAsync(Guid sellerId, CancellationToken cancellationToken = default);
    Task<Order?> GetByConversationIdAsync(Guid conversationId, CancellationToken cancellationToken = default);
}
