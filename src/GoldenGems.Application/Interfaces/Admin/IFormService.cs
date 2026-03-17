using GoldenGems.Application.Common;
using GoldenGems.Application.DTOs.Admin;

namespace GoldenGems.Application.Interfaces.Admin;

public interface IFormService : IBaseService
{
    Task<ApiResponse<FormResponseDto>> CreateAsync(CreateFormRequestDto request, CancellationToken cancellationToken);
    Task<ApiResponse<List<FormResponseDto>>> GetAllAsync(CancellationToken cancellationToken);
    Task<ApiResponse<FormResponseDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<ApiResponse<List<FormResponseDto>>> GetByModuleIdAsync(Guid moduleId, CancellationToken cancellationToken);
    Task<ApiResponse<FormResponseDto>> UpdateAsync(Guid id, CreateFormRequestDto request, CancellationToken cancellationToken);
    Task<ApiResponse<FormResponseDto>> DeleteAsync(Guid id, CancellationToken cancellationToken);
}
