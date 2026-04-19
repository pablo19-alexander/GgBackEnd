using GoldenGems.Application.Common;
using GoldenGems.Application.DTOs.Admin;
using GoldenGems.Domain.Entities.Security;

namespace GoldenGems.Application.Interfaces.Admin;

public interface IActionService : IBaseService
{
    Task<ApiResponse<ActionResponseDto>> CreateActionAsync(CreateActionRequestDto request, CancellationToken cancellationToken);
    Task<ApiResponse<ActionResponseDto>> UpdateActionAsync(Guid id, CreateActionRequestDto request, CancellationToken cancellationToken);
    Task<ApiResponse<ActionResponseDto>> DeleteActionAsync(Guid id, CancellationToken cancellationToken);
    Task<ApiResponse<List<ActionResponseDto>>> GetAllActionsAsync(CancellationToken cancellationToken);
    Task<bool> ActionExistsByCodeAsync(string code, CancellationToken cancellationToken);
    Task<Actions?> GetActionByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Actions?> GetActionByCodeAsync(string code, CancellationToken cancellationToken);
}
