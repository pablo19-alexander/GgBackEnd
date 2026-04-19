namespace GoldenGems.Domain.Interfaces;

public interface IDashboardRepository
{
    Task<DashboardCounts> GetCountsAsync(CancellationToken cancellationToken = default);
}

public class DashboardCounts
{
    public int TotalUsers { get; set; }
    public int ActiveUsers { get; set; }
    public int TotalCompanies { get; set; }
    public int ActiveCompanies { get; set; }
    public int TotalProducts { get; set; }
    public int ActiveProducts { get; set; }
    public int TotalProductTypes { get; set; }
    public int TotalOrders { get; set; }
    public int PendingOrders { get; set; }
    public int PaidOrders { get; set; }
    public int DeliveredOrders { get; set; }
    public int CancelledOrders { get; set; }
    public int TotalPayments { get; set; }
    public int CompletedPayments { get; set; }
    public decimal TotalRevenue { get; set; }
    public int TotalRoles { get; set; }
    public int TotalActions { get; set; }
    public int TotalActionTypes { get; set; }
    public int TotalConversations { get; set; }
}
