using GoldenGems.Application.Common;
using GoldenGems.Application.DTOs.Admin;

namespace GoldenGems.Application.Interfaces.Admin;

public interface IActionTypeService : IBaseService
{
    Task<ApiResponse<ActionTypeResponseDto>> CreateAsync(CreateActionTypeRequestDto request, CancellationToken cancellationToken);
    Task<ApiResponse<ActionTypeResponseDto>> UpdateAsync(Guid id, CreateActionTypeRequestDto request, CancellationToken cancellationToken);
    Task<ApiResponse<ActionTypeResponseDto>> DeleteAsync(Guid id, CancellationToken cancellationToken);
    Task<ApiResponse<List<ActionTypeResponseDto>>> GetAllAsync(CancellationToken cancellationToken);
}
