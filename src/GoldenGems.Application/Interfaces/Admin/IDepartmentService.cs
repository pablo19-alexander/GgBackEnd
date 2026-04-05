using GoldenGems.Application.Common;
using GoldenGems.Application.DTOs.Admin;

namespace GoldenGems.Application.Interfaces.Admin;

public interface IDepartmentService : IBaseService
{
    Task<ApiResponse<List<DepartmentResponseDto>>> GetAllAsync(CancellationToken cancellationToken);
    Task<ApiResponse<DepartmentResponseDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken);
}
