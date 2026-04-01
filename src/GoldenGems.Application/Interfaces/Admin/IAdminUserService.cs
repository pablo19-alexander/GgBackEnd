using GoldenGems.Application.Common;
using GoldenGems.Application.DTOs.Admin;

namespace GoldenGems.Application.Interfaces.Admin;

public interface IAdminUserService : IBaseService
{
    Task<ApiResponse<List<AdminUserResponseDto>>> GetAllUsersAsync(CancellationToken cancellationToken = default);
    Task<ApiResponse<bool>> ChangePasswordAsync(Guid userId, AdminChangePasswordRequestDto request, CancellationToken cancellationToken = default);
    Task<ApiResponse<bool>> UpdateRolesAsync(Guid userId, AdminUpdateRolesRequestDto request, CancellationToken cancellationToken = default);
}
