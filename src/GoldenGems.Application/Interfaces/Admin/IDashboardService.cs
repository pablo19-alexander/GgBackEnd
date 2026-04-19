using GoldenGems.Application.Common;
using GoldenGems.Application.DTOs.Admin;

namespace GoldenGems.Application.Interfaces.Admin;

public interface IDashboardService : IBaseService
{
    Task<ApiResponse<DashboardStatsDto>> GetStatsAsync(CancellationToken cancellationToken);
}
