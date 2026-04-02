using GoldenGems.Domain.Entities.Payment;
using GoldenGems.Domain.Interfaces;
using GoldenGems.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GoldenGems.Infrastructure.Repositories;

public class PaymentRepository : GenericRepository<Payment>, IPaymentRepository
{
    public PaymentRepository(GoldenGemsDbContext context) : base(context) { }

    public async Task<List<Payment>> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        return await _context.Payments.AsNoTracking()
            .Where(p => p.OrderId == orderId && p.IsActive)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<Payment?> GetByTransactionIdAsync(string transactionId, CancellationToken cancellationToken = default)
    {
        return await _context.Payments.AsNoTracking()
            .FirstOrDefaultAsync(p => p.TransactionId == transactionId && p.IsActive, cancellationToken);
    }
}
