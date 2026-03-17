using GoldenGems.Application.Common;
using GoldenGems.Application.DTOs.Admin;

namespace GoldenGems.Application.Interfaces.Admin;

public interface IModuleService : IBaseService
{
    Task<ApiResponse<ModuleResponseDto>> CreateAsync(CreateModuleRequestDto request, CancellationToken cancellationToken);
    Task<ApiResponse<List<ModuleResponseDto>>> GetAllAsync(CancellationToken cancellationToken);
    Task<ApiResponse<ModuleResponseDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<ApiResponse<ModuleResponseDto>> UpdateAsync(Guid id, CreateModuleRequestDto request, CancellationToken cancellationToken);
    Task<ApiResponse<ModuleResponseDto>> DeleteAsync(Guid id, CancellationToken cancellationToken);
}
