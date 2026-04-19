using GoldenGems.Application.Common;
using GoldenGems.Application.DTOs.Admin;
using GoldenGems.Application.Interfaces.Admin;
using GoldenGems.Domain.Entities.Security;
using GoldenGems.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace GoldenGems.Application.Services.Admin;

public class ActionTypeService : BaseService, IActionTypeService
{
    private readonly IActionTypeRepository _actionTypeRepository;

    public ActionTypeService(
        IActionTypeRepository actionTypeRepository,
        ILogger<ActionTypeService> logger)
        : base(logger)
    {
        _actionTypeRepository = actionTypeRepository ?? throw new ArgumentNullException(nameof(actionTypeRepository));
    }

    public async Task<ApiResponse<ActionTypeResponseDto>> CreateAsync(CreateActionTypeRequestDto request, CancellationToken cancellationToken)
    {
        try
        {
            if (request == null)
                return ApiResponse<ActionTypeResponseDto>.ErrorResponse("La solicitud es nula");

            if (string.IsNullOrWhiteSpace(request.Code))
                return ApiResponse<ActionTypeResponseDto>.ErrorResponse("El código es requerido");

            if (string.IsNullOrWhiteSpace(request.Description))
                return ApiResponse<ActionTypeResponseDto>.ErrorResponse("La descripción es requerida");

            var code = request.Code.Trim().ToUpper();

            if (await _actionTypeRepository.ExistsByCodeAsync(code, cancellationToken))
            {
                _logger.LogWarning("Intento de crear tipo de acción con código duplicado: {Code}", code);
                return ApiResponse<ActionTypeResponseDto>.ErrorResponse($"Ya existe un tipo de acción con el código '{code}'");
            }

            var entity = new ActionType
            {
                Id = Guid.NewGuid(),
                Code = code,
                Description = request.Description.Trim(),
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _actionTypeRepository.CreateAsync(entity, cancellationToken);
            _logger.LogInformation("Tipo de acción creado: {Code} (ID: {Id})", created.Code, created.Id);

            return ApiResponse<ActionTypeResponseDto>.SuccessResponse(MapToDto(created), "Tipo de acción creado exitosamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear tipo de acción");
            return ApiResponse<ActionTypeResponseDto>.ErrorResponse("Error al crear el tipo de acción");
        }
    }

    public async Task<ApiResponse<ActionTypeResponseDto>> UpdateAsync(Guid id, CreateActionTypeRequestDto request, CancellationToken cancellationToken)
    {
        try
        {
            var entity = await _actionTypeRepository.GetByIdAsync(id, cancellationToken);
            if (entity == null || !entity.IsActive)
                return ApiResponse<ActionTypeResponseDto>.ErrorResponse("Tipo de acción no encontrado");

            if (string.IsNullOrWhiteSpace(request.Code) || string.IsNullOrWhiteSpace(request.Description))
                return ApiResponse<ActionTypeResponseDto>.ErrorResponse("Código y descripción son requeridos");

            var code = request.Code.Trim().ToUpper();
            if (!string.Equals(entity.Code, code, StringComparison.OrdinalIgnoreCase)
                && await _actionTypeRepository.ExistsByCodeAsync(code, cancellationToken))
            {
                return ApiResponse<ActionTypeResponseDto>.ErrorResponse($"Ya existe un tipo de acción con el código '{code}'");
            }

            entity.Code = code;
            entity.Description = request.Description.Trim();

            var updated = await _actionTypeRepository.UpdateAsync(entity, cancellationToken);
            _logger.LogInformation("Tipo de acción actualizado: {Code} (ID: {Id})", updated.Code, updated.Id);
            return ApiResponse<ActionTypeResponseDto>.SuccessResponse(MapToDto(updated), "Tipo de acción actualizado exitosamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar tipo de acción: {Id}", id);
            return ApiResponse<ActionTypeResponseDto>.ErrorResponse("Error al actualizar el tipo de acción");
        }
    }

    public async Task<ApiResponse<ActionTypeResponseDto>> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var entity = await _actionTypeRepository.GetByIdAsync(id, cancellationToken);
            if (entity == null || !entity.IsActive)
                return ApiResponse<ActionTypeResponseDto>.ErrorResponse("Tipo de acción no encontrado");

            var deleted = await _actionTypeRepository.DeleteAsync(entity, cancellationToken);
            _logger.LogInformation("Tipo de acción desactivado: {Code} (ID: {Id})", deleted.Code, deleted.Id);
            return ApiResponse<ActionTypeResponseDto>.SuccessResponse(MapToDto(deleted), "Tipo de acción eliminado exitosamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar tipo de acción: {Id}", id);
            return ApiResponse<ActionTypeResponseDto>.ErrorResponse("Error al eliminar el tipo de acción");
        }
    }

    public async Task<ApiResponse<List<ActionTypeResponseDto>>> GetAllAsync(CancellationToken cancellationToken)
    {
        try
        {
            var items = await _actionTypeRepository.GetAllAsync(cancellationToken);
            var dtos = items.Select(MapToDto).ToList();
            return ApiResponse<List<ActionTypeResponseDto>>.SuccessResponse(dtos, $"Se encontraron {dtos.Count} tipos de acción");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener tipos de acción");
            return ApiResponse<List<ActionTypeResponseDto>>.ErrorResponse("Error al obtener los tipos de acción");
        }
    }

    private static ActionTypeResponseDto MapToDto(ActionType entity) => new()
    {
        Id = entity.Id,
        Code = entity.Code,
        Description = entity.Description,
        IsActive = entity.IsActive,
        CreatedAt = entity.CreatedAt
    };
}
