using GoldenGems.Application.Common;
using GoldenGems.Application.DTOs.Admin;
using GoldenGems.Application.Interfaces.Admin;
using GoldenGems.Domain.Entities.Security;
using GoldenGems.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace GoldenGems.Application.Services.Admin;

public class RoleService : BaseService, IRoleService
{
    private readonly IRoleRepository _roleRepository;

    public RoleService(IRoleRepository roleRepository, ILogger<RoleService> logger)
        : base(logger)
    {
        _roleRepository = roleRepository ?? throw new ArgumentNullException(nameof(roleRepository));
    }

    public async Task<ApiResponse<RoleResponseDto>> CreateRoleAsync(CreateRoleRequestDto request, CancellationToken cancellationToken)
    {
        try
        {
            if (request == null)
                return ApiResponse<RoleResponseDto>.ErrorResponse("La solicitud es nula");

            if (string.IsNullOrWhiteSpace(request.Name))
                return ApiResponse<RoleResponseDto>.ErrorResponse("El nombre del rol es requerido");

            var name = request.Name.Trim();

            if (await _roleRepository.ExistsByNameAsync(name, cancellationToken))
            {
                _logger.LogWarning("Intento de crear rol con nombre duplicado: {Name}", name);
                return ApiResponse<RoleResponseDto>.ErrorResponse($"Ya existe un rol con el nombre '{name}'");
            }

            var role = new Role
            {
                Id = Guid.NewGuid(),
                Name = name,
                Description = request.Description?.Trim() ?? string.Empty,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var createdRole = await _roleRepository.CreateAsync(role, cancellationToken);
            var roleDto = MapRoleToDto(createdRole);

            _logger.LogInformation("Rol creado exitosamente: {Name} (ID: {Id})", roleDto.Name, roleDto.Id);
            return ApiResponse<RoleResponseDto>.SuccessResponse(roleDto, "Rol creado exitosamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear rol");
            return ApiResponse<RoleResponseDto>.ErrorResponse("Error al crear el rol");
        }
    }

    public async Task<ApiResponse<RoleResponseDto>> UpdateRoleAsync(Guid id, CreateRoleRequestDto request, CancellationToken cancellationToken)
    {
        try
        {
            var role = await _roleRepository.GetByIdAsync(id, cancellationToken);
            if (role == null || !role.IsActive)
                return ApiResponse<RoleResponseDto>.ErrorResponse("Rol no encontrado");

            if (string.IsNullOrWhiteSpace(request.Name))
                return ApiResponse<RoleResponseDto>.ErrorResponse("El nombre del rol es requerido");

            var name = request.Name.Trim();
            if (!string.Equals(role.Name, name, StringComparison.OrdinalIgnoreCase)
                && await _roleRepository.ExistsByNameAsync(name, cancellationToken))
            {
                return ApiResponse<RoleResponseDto>.ErrorResponse($"Ya existe un rol con el nombre '{name}'");
            }

            role.Name = name;
            role.Description = request.Description?.Trim() ?? string.Empty;

            var updated = await _roleRepository.UpdateAsync(role, cancellationToken);
            _logger.LogInformation("Rol actualizado: {Name} (ID: {Id})", updated.Name, updated.Id);
            return ApiResponse<RoleResponseDto>.SuccessResponse(MapRoleToDto(updated), "Rol actualizado exitosamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar rol: {Id}", id);
            return ApiResponse<RoleResponseDto>.ErrorResponse("Error al actualizar el rol");
        }
    }

    public async Task<ApiResponse<RoleResponseDto>> DeleteRoleAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var role = await _roleRepository.GetByIdAsync(id, cancellationToken);
            if (role == null || !role.IsActive)
                return ApiResponse<RoleResponseDto>.ErrorResponse("Rol no encontrado");

            var deleted = await _roleRepository.DeleteAsync(role, cancellationToken);
            _logger.LogInformation("Rol desactivado: {Name} (ID: {Id})", deleted.Name, deleted.Id);
            return ApiResponse<RoleResponseDto>.SuccessResponse(MapRoleToDto(deleted), "Rol eliminado exitosamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar rol: {Id}", id);
            return ApiResponse<RoleResponseDto>.ErrorResponse("Error al eliminar el rol");
        }
    }

    public async Task<ApiResponse<List<RoleResponseDto>>> GetAllRolesAsync(CancellationToken cancellationToken)
    {
        try
        {
            var roles = await _roleRepository.GetAllAsync(cancellationToken);
            var roleDtos = roles.Select(MapRoleToDto).ToList();

            return ApiResponse<List<RoleResponseDto>>.SuccessResponse(
                roleDtos,
                $"Se encontraron {roleDtos.Count} roles"
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener todos los roles");
            return ApiResponse<List<RoleResponseDto>>.ErrorResponse("Error al obtener los roles");
        }
    }

    public async Task<bool> RoleExistsByNameAsync(string name, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(name)) return false;
        return await _roleRepository.ExistsByNameAsync(name, cancellationToken);
    }

    public async Task<Role?> GetRoleByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        if (id == Guid.Empty) return null;
        return await _roleRepository.GetByIdAsync(id, cancellationToken);
    }

    public async Task<Role?> GetRoleByNameAsync(string name, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(name)) return null;
        return await _roleRepository.GetByNameAsync(name, cancellationToken);
    }

    private static RoleResponseDto MapRoleToDto(Role role)
    {
        return new RoleResponseDto
        {
            Id = role.Id,
            Name = role.Name,
            Description = role.Description,
            IsActive = role.IsActive,
            CreatedAt = role.CreatedAt
        };
    }
}
