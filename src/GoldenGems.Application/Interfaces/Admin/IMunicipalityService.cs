using GoldenGems.Application.Common;
using GoldenGems.Application.DTOs.Admin;

namespace GoldenGems.Application.Interfaces.Admin;

public interface IMunicipalityService : IBaseService
{
    Task<ApiResponse<List<MunicipalityResponseDto>>> GetAllAsync(CancellationToken cancellationToken);
    Task<ApiResponse<MunicipalityResponseDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<ApiResponse<List<MunicipalityResponseDto>>> GetByDepartmentIdAsync(Guid departmentId, CancellationToken cancellationToken);
}
