using GoldenGems.Domain.Entities.Payment;
using GoldenGems.Domain.Interfaces;
using GoldenGems.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GoldenGems.Infrastructure.Repositories;

public class DashboardRepository : IDashboardRepository
{
    private readonly GoldenGemsDbContext _context;

    public DashboardRepository(GoldenGemsDbContext context)
    {
        _context = context;
    }

    public async Task<DashboardCounts> GetCountsAsync(CancellationToken cancellationToken = default)
    {
        var counts = new DashboardCounts
        {
            TotalUsers = await _context.Users.CountAsync(cancellationToken),
            ActiveUsers = await _context.Users.CountAsync(u => u.IsActive, cancellationToken),

            TotalCompanies = await _context.Companies.CountAsync(cancellationToken),
            ActiveCompanies = await _context.Companies.CountAsync(c => c.IsActive, cancellationToken),

            TotalProducts = await _context.Products.CountAsync(cancellationToken),
            ActiveProducts = await _context.Products.CountAsync(p => p.IsActive, cancellationToken),
            TotalProductTypes = await _context.ProductTypes.CountAsync(pt => pt.IsActive, cancellationToken),

            TotalOrders = await _context.Orders.CountAsync(cancellationToken),
            PendingOrders = await _context.Orders.CountAsync(o => o.Status == OrderStatus.Pending || o.Status == OrderStatus.PaymentPending, cancellationToken),
            PaidOrders = await _context.Orders.CountAsync(o => o.Status == OrderStatus.Paid, cancellationToken),
            DeliveredOrders = await _context.Orders.CountAsync(o => o.Status == OrderStatus.Delivered, cancellationToken),
            CancelledOrders = await _context.Orders.CountAsync(o => o.Status == OrderStatus.Cancelled, cancellationToken),

            TotalPayments = await _context.Payments.CountAsync(cancellationToken),
            CompletedPayments = await _context.Payments.CountAsync(p => p.Status == PaymentStatus.Completed, cancellationToken),
            TotalRevenue = await _context.Payments
                .Where(p => p.Status == PaymentStatus.Completed)
                .SumAsync(p => (decimal?)p.Amount, cancellationToken) ?? 0m,

            TotalRoles = await _context.Roles.CountAsync(r => r.IsActive, cancellationToken),
            TotalActions = await _context.Actions.CountAsync(a => a.IsActive, cancellationToken),
            TotalActionTypes = await _context.ActionTypes.CountAsync(at => at.IsActive, cancellationToken),

            TotalConversations = await _context.Conversations.CountAsync(cancellationToken),
        };

        return counts;
    }
}
