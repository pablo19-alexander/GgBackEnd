using GoldenGems.Application.Common;
using GoldenGems.Application.DTOs.Admin;
using GoldenGems.Domain.Entities.Security;

namespace GoldenGems.Application.Interfaces.Admin;

public interface IRoleService : IBaseService
{
    Task<ApiResponse<RoleResponseDto>> CreateRoleAsync(CreateRoleRequestDto request, CancellationToken cancellationToken);
    Task<ApiResponse<RoleResponseDto>> UpdateRoleAsync(Guid id, CreateRoleRequestDto request, CancellationToken cancellationToken);
    Task<ApiResponse<RoleResponseDto>> DeleteRoleAsync(Guid id, CancellationToken cancellationToken);
    Task<ApiResponse<List<RoleResponseDto>>> GetAllRolesAsync(CancellationToken cancellationToken);
    Task<bool> RoleExistsByNameAsync(string name, CancellationToken cancellationToken);
    Task<Role?> GetRoleByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Role?> GetRoleByNameAsync(string name, CancellationToken cancellationToken);
}
