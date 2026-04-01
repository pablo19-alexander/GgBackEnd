using GoldenGems.Application.Common;
using GoldenGems.Application.DTOs.Admin;
using GoldenGems.Application.Interfaces.Admin;
using GoldenGems.Application.Interfaces.Auth;
using GoldenGems.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace GoldenGems.Application.Services.Admin;

public class AdminUserService : IAdminUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IPasswordHasherService _passwordHasher;
    private readonly ILogger<AdminUserService> _logger;

    public AdminUserService(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IPasswordHasherService passwordHasher,
        ILogger<AdminUserService> logger)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _passwordHasher = passwordHasher;
        _logger = logger;
    }

    public async Task<ApiResponse<List<AdminUserResponseDto>>> GetAllUsersAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var users = await _userRepository.GetAllWithDetailsAsync(cancellationToken);

            var dtos = users.Select(u => new AdminUserResponseDto
            {
                PersonId = u.Person?.Id ?? Guid.Empty,
                UserId = u.Id,
                Email = u.Email,
                Username = u.Username,
                PasswordHash = u.PasswordHash,
                FirstName = u.Person?.FirstName ?? string.Empty,
                SecondName = u.Person?.SecondName,
                FirstLastName = u.Person?.FirstLastName ?? string.Empty,
                SecondLastName = u.Person?.SecondLastName,
                DocumentTypeId = u.Person?.DocumentTypeId ?? Guid.Empty,
                DocumentTypeName = u.Person?.DocumentType?.Name,
                DocumentNumber = u.Person?.DocumentNumber,
                PhotoUrl = u.Person?.PhotoUrl,
                Roles = u.UserRoles.Where(ur => ur.IsActive && ur.Role != null).Select(ur => ur.Role!.Name).ToList(),
                RoleIds = u.UserRoles.Where(ur => ur.IsActive && ur.Role != null).Select(ur => ur.RoleId).ToList(),
                IsActive = u.IsActive,
                CreatedAt = u.CreatedAt
            }).ToList();

            return ApiResponse<List<AdminUserResponseDto>>.SuccessResponse(dtos, $"Se encontraron {dtos.Count} usuarios");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener todos los usuarios");
            return ApiResponse<List<AdminUserResponseDto>>.ErrorResponse("Error al obtener los usuarios");
        }
    }

    public async Task<ApiResponse<bool>> ChangePasswordAsync(Guid userId, AdminChangePasswordRequestDto request, CancellationToken cancellationToken = default)
    {
        try
        {
            if (request.NewPassword != request.ConfirmPassword)
                return ApiResponse<bool>.ErrorResponse("Las contraseñas no coinciden");

            var user = await _userRepository.GetByIdTrackingAsync(userId, cancellationToken);
            if (user == null)
                return ApiResponse<bool>.ErrorResponse("Usuario no encontrado");

            user.PasswordHash = _passwordHasher.HashPassword(request.NewPassword);
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user, cancellationToken);

            _logger.LogInformation("Contraseña cambiada por admin para usuario {UserId}", userId);
            return ApiResponse<bool>.SuccessResponse(true, "Contraseña actualizada exitosamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cambiar contraseña del usuario {UserId}", userId);
            return ApiResponse<bool>.ErrorResponse("Error al cambiar la contraseña");
        }
    }

    public async Task<ApiResponse<bool>> UpdateRolesAsync(Guid userId, AdminUpdateRolesRequestDto request, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _userRepository.GetByIdTrackingAsync(userId, cancellationToken);
            if (user == null)
                return ApiResponse<bool>.ErrorResponse("Usuario no encontrado");

            var existingRoleIds = await _roleRepository.GetExistingRoleIdsAsync(request.RoleIds, cancellationToken);
            if (existingRoleIds.Count != request.RoleIds.Count)
                return ApiResponse<bool>.ErrorResponse("Uno o más roles no existen");

            await _userRepository.UpdateRolesAsync(userId, request.RoleIds, cancellationToken);

            _logger.LogInformation("Roles actualizados por admin para usuario {UserId}", userId);
            return ApiResponse<bool>.SuccessResponse(true, "Roles actualizados exitosamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar roles del usuario {UserId}", userId);
            return ApiResponse<bool>.ErrorResponse("Error al actualizar los roles");
        }
    }
}
