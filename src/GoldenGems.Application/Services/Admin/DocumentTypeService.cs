using GoldenGems.Application.Common;
using GoldenGems.Application.DTOs.Admin;
using GoldenGems.Application.Interfaces.Admin;
using GoldenGems.Domain.Entities.People;
using GoldenGems.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace GoldenGems.Application.Services.Admin;

public class DocumentTypeService : BaseService, IDocumentTypeService
{
    private readonly IDocumentTypeRepository _documentTypeRepository;

    public DocumentTypeService(IDocumentTypeRepository documentTypeRepository, ILogger<DocumentTypeService> logger)
        : base(logger)
    {
        _documentTypeRepository = documentTypeRepository ?? throw new ArgumentNullException(nameof(documentTypeRepository));
    }

    public async Task<ApiResponse<DocumentTypeResponseDto>> CreateAsync(CreateDocumentTypeRequestDto request, CancellationToken cancellationToken)
    {
        try
        {
            if (request == null)
                return ApiResponse<DocumentTypeResponseDto>.ErrorResponse("La solicitud es nula");

            if (string.IsNullOrWhiteSpace(request.Code))
                return ApiResponse<DocumentTypeResponseDto>.ErrorResponse("El código es requerido");

            if (string.IsNullOrWhiteSpace(request.Name))
                return ApiResponse<DocumentTypeResponseDto>.ErrorResponse("El nombre es requerido");

            var code = request.Code.Trim().ToUpper();

            if (await _documentTypeRepository.ExistsByCodeAsync(code, cancellationToken))
            {
                _logger.LogWarning("Intento de crear tipo de documento con código duplicado: {Code}", code);
                return ApiResponse<DocumentTypeResponseDto>.ErrorResponse($"Ya existe un tipo de documento con el código '{code}'");
            }

            var documentType = new DocumentType
            {
                Id = Guid.NewGuid(),
                Code = code,
                Name = request.Name.Trim(),
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _documentTypeRepository.CreateAsync(documentType, cancellationToken);
            var dto = MapToDto(created);

            _logger.LogInformation("Tipo de documento creado exitosamente: {Code} (ID: {Id})", dto.Code, dto.Id);
            return ApiResponse<DocumentTypeResponseDto>.SuccessResponse(dto, "Tipo de documento creado exitosamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear tipo de documento");
            return ApiResponse<DocumentTypeResponseDto>.ErrorResponse("Error al crear el tipo de documento");
        }
    }

    public async Task<ApiResponse<List<DocumentTypeResponseDto>>> GetAllAsync(CancellationToken cancellationToken)
    {
        try
        {
            var documentTypes = await _documentTypeRepository.GetAllActiveAsync(cancellationToken);
            var dtos = documentTypes.Select(MapToDto).ToList();

            return ApiResponse<List<DocumentTypeResponseDto>>.SuccessResponse(
                dtos,
                $"Se encontraron {dtos.Count} tipos de documento"
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener todos los tipos de documento");
            return ApiResponse<List<DocumentTypeResponseDto>>.ErrorResponse("Error al obtener los tipos de documento");
        }
    }

    public async Task<ApiResponse<DocumentTypeResponseDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            if (id == Guid.Empty)
                return ApiResponse<DocumentTypeResponseDto>.ErrorResponse("El ID es inválido");

            var documentType = await _documentTypeRepository.GetByIdAsync(id, cancellationToken);

            if (documentType == null || !documentType.IsActive)
                return ApiResponse<DocumentTypeResponseDto>.ErrorResponse("Tipo de documento no encontrado");

            return ApiResponse<DocumentTypeResponseDto>.SuccessResponse(MapToDto(documentType), "Tipo de documento encontrado");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener tipo de documento por ID: {Id}", id);
            return ApiResponse<DocumentTypeResponseDto>.ErrorResponse("Error al obtener el tipo de documento");
        }
    }

    public async Task<ApiResponse<DocumentTypeResponseDto>> UpdateAsync(Guid id, CreateDocumentTypeRequestDto request, CancellationToken cancellationToken)
    {
        try
        {
            if (id == Guid.Empty)
                return ApiResponse<DocumentTypeResponseDto>.ErrorResponse("El ID es inválido");

            if (request == null)
                return ApiResponse<DocumentTypeResponseDto>.ErrorResponse("La solicitud es nula");

            var documentType = await _documentTypeRepository.GetByIdAsync(id, cancellationToken);

            if (documentType == null || !documentType.IsActive)
                return ApiResponse<DocumentTypeResponseDto>.ErrorResponse("Tipo de documento no encontrado");

            var code = request.Code.Trim().ToUpper();

            // Verificar unicidad del código solo si cambió
            if (!string.Equals(documentType.Code, code, StringComparison.OrdinalIgnoreCase))
            {
                if (await _documentTypeRepository.ExistsByCodeAsync(code, cancellationToken))
                {
                    _logger.LogWarning("Intento de actualizar tipo de documento con código duplicado: {Code}", code);
                    return ApiResponse<DocumentTypeResponseDto>.ErrorResponse($"Ya existe un tipo de documento con el código '{code}'");
                }
            }

            documentType.Code = code;
            documentType.Name = request.Name.Trim();

            var updated = await _documentTypeRepository.UpdateAsync(documentType, cancellationToken);
            var dto = MapToDto(updated);

            _logger.LogInformation("Tipo de documento actualizado exitosamente: {Code} (ID: {Id})", dto.Code, dto.Id);
            return ApiResponse<DocumentTypeResponseDto>.SuccessResponse(dto, "Tipo de documento actualizado exitosamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar tipo de documento: {Id}", id);
            return ApiResponse<DocumentTypeResponseDto>.ErrorResponse("Error al actualizar el tipo de documento");
        }
    }

    public async Task<ApiResponse<DocumentTypeResponseDto>> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            if (id == Guid.Empty)
                return ApiResponse<DocumentTypeResponseDto>.ErrorResponse("El ID es inválido");

            var documentType = await _documentTypeRepository.GetByIdAsync(id, cancellationToken);

            if (documentType == null || !documentType.IsActive)
                return ApiResponse<DocumentTypeResponseDto>.ErrorResponse("Tipo de documento no encontrado");

            var deleted = await _documentTypeRepository.DeleteAsync(documentType, cancellationToken);
            var dto = MapToDto(deleted);

            _logger.LogInformation("Tipo de documento eliminado exitosamente: {Code} (ID: {Id})", dto.Code, dto.Id);
            return ApiResponse<DocumentTypeResponseDto>.SuccessResponse(dto, "Tipo de documento eliminado exitosamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar tipo de documento: {Id}", id);
            return ApiResponse<DocumentTypeResponseDto>.ErrorResponse("Error al eliminar el tipo de documento");
        }
    }

    private static DocumentTypeResponseDto MapToDto(DocumentType documentType)
    {
        return new DocumentTypeResponseDto
        {
            Id = documentType.Id,
            Code = documentType.Code,
            Name = documentType.Name,
            IsActive = documentType.IsActive,
            CreatedAt = documentType.CreatedAt
        };
    }
}
