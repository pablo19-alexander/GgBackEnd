using GoldenGems.Application.Common;
using GoldenGems.Application.DTOs.Admin;

namespace GoldenGems.Application.Interfaces.Admin;

public interface IRegionService : IBaseService
{
    Task<ApiResponse<List<RegionResponseDto>>> GetAllAsync(CancellationToken cancellationToken);
    Task<ApiResponse<RegionResponseDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<ApiResponse<List<RegionResponseDto>>> GetByDepartmentAsync(string department, CancellationToken cancellationToken);
    Task<ApiResponse<List<string>>> GetDepartmentsAsync(CancellationToken cancellationToken);
}
