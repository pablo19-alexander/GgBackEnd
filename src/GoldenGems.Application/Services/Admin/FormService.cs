using GoldenGems.Application.Common;
using GoldenGems.Application.DTOs.Admin;
using GoldenGems.Application.Interfaces.Admin;
using GoldenGems.Domain.Entities.Security;
using GoldenGems.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace GoldenGems.Application.Services.Admin;

public class FormService : BaseService, IFormService
{
    private readonly IFormRepository _formRepository;
    private readonly IModuleRepository _moduleRepository;

    public FormService(
        IFormRepository formRepository,
        IModuleRepository moduleRepository,
        ILogger<FormService> logger)
        : base(logger)
    {
        _formRepository = formRepository ?? throw new ArgumentNullException(nameof(formRepository));
        _moduleRepository = moduleRepository ?? throw new ArgumentNullException(nameof(moduleRepository));
    }

    public async Task<ApiResponse<FormResponseDto>> CreateAsync(CreateFormRequestDto request, CancellationToken cancellationToken)
    {
        try
        {
            if (request == null)
                return ApiResponse<FormResponseDto>.ErrorResponse("La solicitud es nula");

            if (string.IsNullOrWhiteSpace(request.Code))
                return ApiResponse<FormResponseDto>.ErrorResponse("El código del formulario es requerido");

            if (string.IsNullOrWhiteSpace(request.Name))
                return ApiResponse<FormResponseDto>.ErrorResponse("El nombre del formulario es requerido");

            if (string.IsNullOrWhiteSpace(request.FormReference))
                return ApiResponse<FormResponseDto>.ErrorResponse("La referencia del formulario es requerida");

            if (request.ModuleId == Guid.Empty)
                return ApiResponse<FormResponseDto>.ErrorResponse("El módulo es requerido");

            var code = request.Code.Trim().ToUpper();
            var name = request.Name.Trim();

            // Validar que el módulo exista
            var moduleExists = await _moduleRepository.ExistsAsync(request.ModuleId, cancellationToken);
            if (!moduleExists)
            {
                _logger.LogWarning("Intento de crear formulario con módulo inexistente: {ModuleId}", request.ModuleId);
                return ApiResponse<FormResponseDto>.ErrorResponse("El módulo especificado no existe");
            }

            if (await _formRepository.ExistsByCodeAsync(code, cancellationToken))
            {
                _logger.LogWarning("Intento de crear formulario con código duplicado: {Code}", code);
                return ApiResponse<FormResponseDto>.ErrorResponse($"Ya existe un formulario con el código '{code}'");
            }

            var form = new Form
            {
                Id = Guid.NewGuid(),
                Code = code,
                FormReference = request.FormReference.Trim(),
                Name = name,
                Description = request.Description?.Trim() ?? string.Empty,
                Route = request.Route?.Trim(),
                ModuleId = request.ModuleId,
                DisplayOrder = request.DisplayOrder,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var createdForm = await _formRepository.CreateAsync(form, cancellationToken);
            var dto = MapToDto(createdForm);

            _logger.LogInformation("Formulario creado exitosamente: {Code} (ID: {Id})", form.Code, form.Id);
            return ApiResponse<FormResponseDto>.SuccessResponse(dto, "Formulario creado exitosamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear formulario");
            return ApiResponse<FormResponseDto>.ErrorResponse("Error al crear el formulario");
        }
    }

    public async Task<ApiResponse<List<FormResponseDto>>> GetAllAsync(CancellationToken cancellationToken)
    {
        try
        {
            var forms = await _formRepository.GetAllAsync(cancellationToken);
            var dtos = forms.Select(MapToDto).ToList();

            return ApiResponse<List<FormResponseDto>>.SuccessResponse(
                dtos,
                $"Se encontraron {dtos.Count} formularios"
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener todos los formularios");
            return ApiResponse<List<FormResponseDto>>.ErrorResponse("Error al obtener los formularios");
        }
    }

    public async Task<ApiResponse<FormResponseDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            if (id == Guid.Empty)
                return ApiResponse<FormResponseDto>.ErrorResponse("El ID del formulario es requerido");

            var form = await _formRepository.GetByIdAsync(id, cancellationToken);
            if (form == null)
                return ApiResponse<FormResponseDto>.ErrorResponse("Formulario no encontrado");

            return ApiResponse<FormResponseDto>.SuccessResponse(MapToDto(form), "Formulario encontrado");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener formulario por ID: {Id}", id);
            return ApiResponse<FormResponseDto>.ErrorResponse("Error al obtener el formulario");
        }
    }

    public async Task<ApiResponse<List<FormResponseDto>>> GetByModuleIdAsync(Guid moduleId, CancellationToken cancellationToken)
    {
        try
        {
            if (moduleId == Guid.Empty)
                return ApiResponse<List<FormResponseDto>>.ErrorResponse("El ID del módulo es requerido");

            var moduleExists = await _moduleRepository.ExistsAsync(moduleId, cancellationToken);
            if (!moduleExists)
                return ApiResponse<List<FormResponseDto>>.ErrorResponse("El módulo especificado no existe");

            var forms = await _formRepository.GetByModuleIdAsync(moduleId, cancellationToken);
            var dtos = forms.Select(MapToDto).ToList();

            return ApiResponse<List<FormResponseDto>>.SuccessResponse(
                dtos,
                $"Se encontraron {dtos.Count} formularios para el módulo"
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener formularios por módulo: {ModuleId}", moduleId);
            return ApiResponse<List<FormResponseDto>>.ErrorResponse("Error al obtener los formularios");
        }
    }

    public async Task<ApiResponse<FormResponseDto>> UpdateAsync(Guid id, CreateFormRequestDto request, CancellationToken cancellationToken)
    {
        try
        {
            if (id == Guid.Empty)
                return ApiResponse<FormResponseDto>.ErrorResponse("El ID del formulario es requerido");

            if (request == null)
                return ApiResponse<FormResponseDto>.ErrorResponse("La solicitud es nula");

            var form = await _formRepository.GetByIdAsync(id, cancellationToken);
            if (form == null)
                return ApiResponse<FormResponseDto>.ErrorResponse("Formulario no encontrado");

            var code = request.Code.Trim().ToUpper();

            // Verificar que el módulo exista
            if (request.ModuleId == Guid.Empty)
                return ApiResponse<FormResponseDto>.ErrorResponse("El módulo es requerido");

            var moduleExists = await _moduleRepository.ExistsAsync(request.ModuleId, cancellationToken);
            if (!moduleExists)
                return ApiResponse<FormResponseDto>.ErrorResponse("El módulo especificado no existe");

            // Verificar que el código no esté en uso por otro formulario
            if (!form.Code.Equals(code, StringComparison.OrdinalIgnoreCase))
            {
                if (await _formRepository.ExistsByCodeAsync(code, cancellationToken))
                {
                    _logger.LogWarning("Intento de actualizar formulario con código duplicado: {Code}", code);
                    return ApiResponse<FormResponseDto>.ErrorResponse($"Ya existe un formulario con el código '{code}'");
                }
            }

            form.Code = code;
            form.FormReference = request.FormReference.Trim();
            form.Name = request.Name.Trim();
            form.Description = request.Description?.Trim() ?? string.Empty;
            form.Route = request.Route?.Trim();
            form.ModuleId = request.ModuleId;
            form.DisplayOrder = request.DisplayOrder;

            var updatedForm = await _formRepository.UpdateAsync(form, cancellationToken);
            var dto = MapToDto(updatedForm);

            _logger.LogInformation("Formulario actualizado exitosamente: {Code} (ID: {Id})", form.Code, form.Id);
            return ApiResponse<FormResponseDto>.SuccessResponse(dto, "Formulario actualizado exitosamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar formulario: {Id}", id);
            return ApiResponse<FormResponseDto>.ErrorResponse("Error al actualizar el formulario");
        }
    }

    public async Task<ApiResponse<FormResponseDto>> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            if (id == Guid.Empty)
                return ApiResponse<FormResponseDto>.ErrorResponse("El ID del formulario es requerido");

            var form = await _formRepository.GetByIdAsync(id, cancellationToken);
            if (form == null)
                return ApiResponse<FormResponseDto>.ErrorResponse("Formulario no encontrado");

            var deletedForm = await _formRepository.DeleteAsync(form, cancellationToken);
            var dto = MapToDto(deletedForm);

            _logger.LogInformation("Formulario eliminado (soft delete) exitosamente: {Code} (ID: {Id})", form.Code, form.Id);
            return ApiResponse<FormResponseDto>.SuccessResponse(dto, "Formulario eliminado exitosamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar formulario: {Id}", id);
            return ApiResponse<FormResponseDto>.ErrorResponse("Error al eliminar el formulario");
        }
    }

    private static FormResponseDto MapToDto(Form form)
    {
        return new FormResponseDto
        {
            Id = form.Id,
            Code = form.Code,
            FormReference = form.FormReference,
            Name = form.Name,
            Description = form.Description,
            Route = form.Route,
            ModuleId = form.ModuleId,
            DisplayOrder = form.DisplayOrder,
            IsActive = form.IsActive,
            CreatedAt = form.CreatedAt
        };
    }
}
