using GoldenGems.Domain.Entities.Payment;
using GoldenGems.Domain.Interfaces;
using GoldenGems.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GoldenGems.Infrastructure.Repositories;

public class OrderRepository : GenericRepository<Order>, IOrderRepository
{
    public OrderRepository(GoldenGemsDbContext context) : base(context) { }

    public async Task<Order?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Orders.AsNoTracking()
            .Include(o => o.Buyer)
            .Include(o => o.Seller)
            .Include(o => o.Product)
            .Include(o => o.Company)
            .Include(o => o.Payments.Where(p => p.IsActive))
            .FirstOrDefaultAsync(o => o.Id == id && o.IsActive, cancellationToken);
    }

    public async Task<List<Order>> GetByBuyerIdAsync(Guid buyerId, CancellationToken cancellationToken = default)
    {
        return await _context.Orders.AsNoTracking()
            .Include(o => o.Product)
            .Include(o => o.Company)
            .Where(o => o.BuyerId == buyerId && o.IsActive)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Order>> GetBySellerIdAsync(Guid sellerId, CancellationToken cancellationToken = default)
    {
        return await _context.Orders.AsNoTracking()
            .Include(o => o.Product)
            .Include(o => o.Buyer)
            .Where(o => o.SellerId == sellerId && o.IsActive)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<Order?> GetByConversationIdAsync(Guid conversationId, CancellationToken cancellationToken = default)
    {
        return await _context.Orders.AsNoTracking()
            .FirstOrDefaultAsync(o => o.ConversationId == conversationId && o.IsActive, cancellationToken);
    }
}
