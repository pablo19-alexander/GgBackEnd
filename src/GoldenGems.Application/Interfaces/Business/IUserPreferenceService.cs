using GoldenGems.Application.Common;
using GoldenGems.Application.DTOs.Business;

namespace GoldenGems.Application.Interfaces.Business;

public interface IUserPreferenceService : IBaseService
{
    Task<ApiResponse<PreferencesResponseDto>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken);
    Task<ApiResponse<PreferencesResponseDto>> UpdateAsync(Guid userId, UpdatePreferencesRequestDto request, CancellationToken cancellationToken);
    Task<ApiResponse<PreferencesResponseDto>> DeleteAsync(Guid userId, CancellationToken cancellationToken);
}
