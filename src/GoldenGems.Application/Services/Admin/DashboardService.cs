using GoldenGems.Application.Common;
using GoldenGems.Application.DTOs.Admin;
using GoldenGems.Application.Interfaces.Admin;
using GoldenGems.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace GoldenGems.Application.Services.Admin;

public class DashboardService : BaseService, IDashboardService
{
    private readonly IDashboardRepository _dashboardRepository;

    public DashboardService(IDashboardRepository dashboardRepository, ILogger<DashboardService> logger)
        : base(logger)
    {
        _dashboardRepository = dashboardRepository ?? throw new ArgumentNullException(nameof(dashboardRepository));
    }

    public async Task<ApiResponse<DashboardStatsDto>> GetStatsAsync(CancellationToken cancellationToken)
    {
        try
        {
            var counts = await _dashboardRepository.GetCountsAsync(cancellationToken);
            var dto = new DashboardStatsDto
            {
                TotalUsers = counts.TotalUsers,
                ActiveUsers = counts.ActiveUsers,
                TotalCompanies = counts.TotalCompanies,
                ActiveCompanies = counts.ActiveCompanies,
                TotalProducts = counts.TotalProducts,
                ActiveProducts = counts.ActiveProducts,
                TotalProductTypes = counts.TotalProductTypes,
                TotalOrders = counts.TotalOrders,
                PendingOrders = counts.PendingOrders,
                PaidOrders = counts.PaidOrders,
                DeliveredOrders = counts.DeliveredOrders,
                CancelledOrders = counts.CancelledOrders,
                TotalPayments = counts.TotalPayments,
                CompletedPayments = counts.CompletedPayments,
                TotalRevenue = counts.TotalRevenue,
                TotalRoles = counts.TotalRoles,
                TotalActions = counts.TotalActions,
                TotalActionTypes = counts.TotalActionTypes,
                TotalConversations = counts.TotalConversations,
            };
            return ApiResponse<DashboardStatsDto>.SuccessResponse(dto, "Estadísticas obtenidas");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener estadísticas del dashboard");
            return ApiResponse<DashboardStatsDto>.ErrorResponse("Error al obtener estadísticas");
        }
    }
}
