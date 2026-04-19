using GoldenGems.Application.Common;
using GoldenGems.Application.DTOs.Admin;
using GoldenGems.Application.Interfaces.Admin;
using GoldenGems.Domain.Entities.Security;
using GoldenGems.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace GoldenGems.Application.Services.Admin;

public class ActionService : BaseService, IActionService
{
    private readonly IActionRepository _actionRepository;
    private readonly IActionTypeRepository _actionTypeRepository;

    public ActionService(
        IActionRepository actionRepository,
        IActionTypeRepository actionTypeRepository,
        ILogger<ActionService> logger)
        : base(logger)
    {
        _actionRepository = actionRepository ?? throw new ArgumentNullException(nameof(actionRepository));
        _actionTypeRepository = actionTypeRepository ?? throw new ArgumentNullException(nameof(actionTypeRepository));
    }

    public async Task<ApiResponse<ActionResponseDto>> CreateActionAsync(CreateActionRequestDto request, CancellationToken cancellationToken)
    {
        try
        {
            if (request == null)
                return ApiResponse<ActionResponseDto>.ErrorResponse("La solicitud es nula");

            if (string.IsNullOrWhiteSpace(request.Code))
                return ApiResponse<ActionResponseDto>.ErrorResponse("El código de la acción es requerido");

            if (string.IsNullOrWhiteSpace(request.Name))
                return ApiResponse<ActionResponseDto>.ErrorResponse("El nombre de la acción es requerido");

            var code = request.Code.Trim().ToUpper();
            var name = request.Name.Trim();

            if (request.ActionTypeId == Guid.Empty)
                return ApiResponse<ActionResponseDto>.ErrorResponse("El tipo de acción es requerido");

            var actionType = await _actionTypeRepository.GetByIdAsync(request.ActionTypeId, cancellationToken);
            if (actionType is null)
            {
                _logger.LogWarning("Intento de crear acción con tipo inexistente: {ActionTypeId}", request.ActionTypeId);
                return ApiResponse<ActionResponseDto>.ErrorResponse("El tipo de acción especificado no existe");
            }

            if (await _actionRepository.ExistsByCodeAsync(code, cancellationToken))
            {
                _logger.LogWarning("Intento de crear acción con código duplicado: {Code}", code);
                return ApiResponse<ActionResponseDto>.ErrorResponse($"Ya existe una acción con el código '{code}'");
            }

            var action = new Actions
            {
                Id = Guid.NewGuid(),
                Name = name,
                Code = code,
                Description = request.Description?.Trim() ?? string.Empty,
                ActionTypeId = actionType.Id,
                ActionType = actionType,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var createdAction = await _actionRepository.CreateAsync(action, cancellationToken);
            createdAction.ActionType = actionType;

            var actionDto = MapActionToDto(createdAction);
            _logger.LogInformation("Acción creada exitosamente: {Code} (ID: {Id})", action.Code, action.Id);
            return ApiResponse<ActionResponseDto>.SuccessResponse(actionDto, "Acción creada exitosamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear acción");
            return ApiResponse<ActionResponseDto>.ErrorResponse("Error al crear la acción");
        }
    }

    public async Task<ApiResponse<ActionResponseDto>> UpdateActionAsync(Guid id, CreateActionRequestDto request, CancellationToken cancellationToken)
    {
        try
        {
            var action = await _actionRepository.GetByIdAsync(id, cancellationToken);
            if (action == null || !action.IsActive)
                return ApiResponse<ActionResponseDto>.ErrorResponse("Acción no encontrada");

            if (string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(request.Code))
                return ApiResponse<ActionResponseDto>.ErrorResponse("Nombre y código son requeridos");

            if (request.ActionTypeId == Guid.Empty)
                return ApiResponse<ActionResponseDto>.ErrorResponse("El tipo de acción es requerido");

            var actionType = await _actionTypeRepository.GetByIdAsync(request.ActionTypeId, cancellationToken);
            if (actionType is null)
                return ApiResponse<ActionResponseDto>.ErrorResponse("El tipo de acción especificado no existe");

            var code = request.Code.Trim().ToUpper();
            if (!string.Equals(action.Code, code, StringComparison.OrdinalIgnoreCase)
                && await _actionRepository.ExistsByCodeAsync(code, cancellationToken))
            {
                return ApiResponse<ActionResponseDto>.ErrorResponse($"Ya existe una acción con el código '{code}'");
            }

            action.Name = request.Name.Trim();
            action.Code = code;
            action.Description = request.Description?.Trim() ?? string.Empty;
            action.ActionTypeId = actionType.Id;
            action.ActionType = actionType;

            var updated = await _actionRepository.UpdateAsync(action, cancellationToken);
            updated.ActionType = actionType;
            _logger.LogInformation("Acción actualizada: {Code} (ID: {Id})", updated.Code, updated.Id);
            return ApiResponse<ActionResponseDto>.SuccessResponse(MapActionToDto(updated), "Acción actualizada exitosamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar acción: {Id}", id);
            return ApiResponse<ActionResponseDto>.ErrorResponse("Error al actualizar la acción");
        }
    }

    public async Task<ApiResponse<ActionResponseDto>> DeleteActionAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var action = await _actionRepository.GetByIdAsync(id, cancellationToken);
            if (action == null || !action.IsActive)
                return ApiResponse<ActionResponseDto>.ErrorResponse("Acción no encontrada");

            var deleted = await _actionRepository.DeleteAsync(action, cancellationToken);
            _logger.LogInformation("Acción desactivada: {Code} (ID: {Id})", deleted.Code, deleted.Id);
            return ApiResponse<ActionResponseDto>.SuccessResponse(MapActionToDto(deleted), "Acción eliminada exitosamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar acción: {Id}", id);
            return ApiResponse<ActionResponseDto>.ErrorResponse("Error al eliminar la acción");
        }
    }

    public async Task<ApiResponse<List<ActionResponseDto>>> GetAllActionsAsync(CancellationToken cancellationToken)
    {
        try
        {
            var actions = await _actionRepository.GetAllAsync(cancellationToken);
            var actionDtos = actions.Select(MapActionToDto).ToList();

            return ApiResponse<List<ActionResponseDto>>.SuccessResponse(
                actionDtos,
                $"Se encontraron {actionDtos.Count} acciones"
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener todas las acciones");
            return ApiResponse<List<ActionResponseDto>>.ErrorResponse("Error al obtener las acciones");
        }
    }

    public async Task<bool> ActionExistsByCodeAsync(string code, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(code)) return false;
        return await _actionRepository.ExistsByCodeAsync(code, cancellationToken);
    }

    public async Task<Actions?> GetActionByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        if (id == Guid.Empty) return null;
        return await _actionRepository.GetByIdAsync(id, cancellationToken);
    }

    public async Task<Actions?> GetActionByCodeAsync(string code, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(code)) return null;
        return await _actionRepository.GetByCodeAsync(code, cancellationToken);
    }

    private static ActionResponseDto MapActionToDto(Actions action)
    {
        return new ActionResponseDto
        {
            Id = action.Id,
            Name = action.Name,
            Code = action.Code,
            Description = action.Description,
            ActionTypeId = action.ActionTypeId,
            ActionTypeCode = action.ActionType?.Code ?? string.Empty,
            ActionTypeDescription = action.ActionType?.Description,
            IsActive = action.IsActive,
            CreatedAt = action.CreatedAt
        };
    }
}
