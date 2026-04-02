using GoldenGems.Domain.Entities.Payment;

namespace GoldenGems.Domain.Interfaces;

public interface IPaymentRepository : IRepository<Payment>
{
    Task<List<Payment>> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default);
    Task<Payment?> GetByTransactionIdAsync(string transactionId, CancellationToken cancellationToken = default);
}
