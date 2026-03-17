using GoldenGems.Application.Common;
using GoldenGems.Application.DTOs.Admin;
using GoldenGems.Application.Interfaces.Admin;
using GoldenGems.Domain.Entities.Security;
using GoldenGems.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace GoldenGems.Application.Services.Admin;

public class ModuleService : BaseService, IModuleService
{
    private readonly IModuleRepository _moduleRepository;

    public ModuleService(
        IModuleRepository moduleRepository,
        ILogger<ModuleService> logger)
        : base(logger)
    {
        _moduleRepository = moduleRepository ?? throw new ArgumentNullException(nameof(moduleRepository));
    }

    public async Task<ApiResponse<ModuleResponseDto>> CreateAsync(CreateModuleRequestDto request, CancellationToken cancellationToken)
    {
        try
        {
            if (request == null)
                return ApiResponse<ModuleResponseDto>.ErrorResponse("La solicitud es nula");

            if (string.IsNullOrWhiteSpace(request.Code))
                return ApiResponse<ModuleResponseDto>.ErrorResponse("El código del módulo es requerido");

            if (string.IsNullOrWhiteSpace(request.Name))
                return ApiResponse<ModuleResponseDto>.ErrorResponse("El nombre del módulo es requerido");

            var code = request.Code.Trim().ToUpper();
            var name = request.Name.Trim();

            if (await _moduleRepository.ExistsByCodeAsync(code, cancellationToken))
            {
                _logger.LogWarning("Intento de crear módulo con código duplicado: {Code}", code);
                return ApiResponse<ModuleResponseDto>.ErrorResponse($"Ya existe un módulo con el código '{code}'");
            }

            var module = new Module
            {
                Id = Guid.NewGuid(),
                Code = code,
                Name = name,
                Description = request.Description?.Trim() ?? string.Empty,
                Icon = request.Icon?.Trim(),
                DisplayOrder = request.DisplayOrder,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var createdModule = await _moduleRepository.CreateAsync(module, cancellationToken);
            var dto = MapToDto(createdModule);

            _logger.LogInformation("Módulo creado exitosamente: {Code} (ID: {Id})", module.Code, module.Id);
            return ApiResponse<ModuleResponseDto>.SuccessResponse(dto, "Módulo creado exitosamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear módulo");
            return ApiResponse<ModuleResponseDto>.ErrorResponse("Error al crear el módulo");
        }
    }

    public async Task<ApiResponse<List<ModuleResponseDto>>> GetAllAsync(CancellationToken cancellationToken)
    {
        try
        {
            var modules = await _moduleRepository.GetAllAsync(cancellationToken);
            var dtos = modules.Select(MapToDto).ToList();

            return ApiResponse<List<ModuleResponseDto>>.SuccessResponse(
                dtos,
                $"Se encontraron {dtos.Count} módulos"
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener todos los módulos");
            return ApiResponse<List<ModuleResponseDto>>.ErrorResponse("Error al obtener los módulos");
        }
    }

    public async Task<ApiResponse<ModuleResponseDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            if (id == Guid.Empty)
                return ApiResponse<ModuleResponseDto>.ErrorResponse("El ID del módulo es requerido");

            var module = await _moduleRepository.GetByIdAsync(id, cancellationToken);
            if (module == null)
                return ApiResponse<ModuleResponseDto>.ErrorResponse("Módulo no encontrado");

            return ApiResponse<ModuleResponseDto>.SuccessResponse(MapToDto(module), "Módulo encontrado");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener módulo por ID: {Id}", id);
            return ApiResponse<ModuleResponseDto>.ErrorResponse("Error al obtener el módulo");
        }
    }

    public async Task<ApiResponse<ModuleResponseDto>> UpdateAsync(Guid id, CreateModuleRequestDto request, CancellationToken cancellationToken)
    {
        try
        {
            if (id == Guid.Empty)
                return ApiResponse<ModuleResponseDto>.ErrorResponse("El ID del módulo es requerido");

            if (request == null)
                return ApiResponse<ModuleResponseDto>.ErrorResponse("La solicitud es nula");

            var module = await _moduleRepository.GetByIdAsync(id, cancellationToken);
            if (module == null)
                return ApiResponse<ModuleResponseDto>.ErrorResponse("Módulo no encontrado");

            var code = request.Code.Trim().ToUpper();

            // Verificar que el código no esté en uso por otro módulo
            if (!module.Code.Equals(code, StringComparison.OrdinalIgnoreCase))
            {
                if (await _moduleRepository.ExistsByCodeAsync(code, cancellationToken))
                {
                    _logger.LogWarning("Intento de actualizar módulo con código duplicado: {Code}", code);
                    return ApiResponse<ModuleResponseDto>.ErrorResponse($"Ya existe un módulo con el código '{code}'");
                }
            }

            module.Code = code;
            module.Name = request.Name.Trim();
            module.Description = request.Description?.Trim() ?? string.Empty;
            module.Icon = request.Icon?.Trim();
            module.DisplayOrder = request.DisplayOrder;

            var updatedModule = await _moduleRepository.UpdateAsync(module, cancellationToken);
            var dto = MapToDto(updatedModule);

            _logger.LogInformation("Módulo actualizado exitosamente: {Code} (ID: {Id})", module.Code, module.Id);
            return ApiResponse<ModuleResponseDto>.SuccessResponse(dto, "Módulo actualizado exitosamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar módulo: {Id}", id);
            return ApiResponse<ModuleResponseDto>.ErrorResponse("Error al actualizar el módulo");
        }
    }

    public async Task<ApiResponse<ModuleResponseDto>> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            if (id == Guid.Empty)
                return ApiResponse<ModuleResponseDto>.ErrorResponse("El ID del módulo es requerido");

            var module = await _moduleRepository.GetByIdAsync(id, cancellationToken);
            if (module == null)
                return ApiResponse<ModuleResponseDto>.ErrorResponse("Módulo no encontrado");

            var deletedModule = await _moduleRepository.DeleteAsync(module, cancellationToken);
            var dto = MapToDto(deletedModule);

            _logger.LogInformation("Módulo eliminado (soft delete) exitosamente: {Code} (ID: {Id})", module.Code, module.Id);
            return ApiResponse<ModuleResponseDto>.SuccessResponse(dto, "Módulo eliminado exitosamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar módulo: {Id}", id);
            return ApiResponse<ModuleResponseDto>.ErrorResponse("Error al eliminar el módulo");
        }
    }

    private static ModuleResponseDto MapToDto(Module module)
    {
        return new ModuleResponseDto
        {
            Id = module.Id,
            Code = module.Code,
            Name = module.Name,
            Description = module.Description,
            Icon = module.Icon,
            DisplayOrder = module.DisplayOrder,
            IsActive = module.IsActive,
            CreatedAt = module.CreatedAt
        };
    }
}
