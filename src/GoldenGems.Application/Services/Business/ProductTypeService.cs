using GoldenGems.Application.Common;
using GoldenGems.Application.DTOs.Business;
using GoldenGems.Application.Interfaces.Business;
using GoldenGems.Domain.Entities.Business;
using GoldenGems.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace GoldenGems.Application.Services.Business;

public class ProductTypeService : BaseService, IProductTypeService
{
    private readonly IProductTypeRepository _productTypeRepository;

    public ProductTypeService(IProductTypeRepository productTypeRepository, ILogger<ProductTypeService> logger) : base(logger)
    {
        _productTypeRepository = productTypeRepository;
    }

    public async Task<ApiResponse<ProductTypeResponseDto>> CreateAsync(CreateProductTypeRequestDto request, CancellationToken cancellationToken)
    {
        try
        {
            var code = request.Code.Trim().ToUpper();
            if (await _productTypeRepository.ExistsByCodeAsync(code, cancellationToken))
                return ApiResponse<ProductTypeResponseDto>.ErrorResponse($"Ya existe un tipo de producto con el código '{code}'.");

            var entity = new ProductType
            {
                Name = request.Name.Trim(),
                Code = code,
                Description = request.Description?.Trim() ?? string.Empty,
                Icon = request.Icon?.Trim() ?? string.Empty
            };

            var created = await _productTypeRepository.CreateAsync(entity, cancellationToken);
            return ApiResponse<ProductTypeResponseDto>.SuccessResponse(MapToDto(created), "Tipo de producto creado exitosamente.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear tipo de producto");
            return ApiResponse<ProductTypeResponseDto>.ErrorResponse("Error al crear el tipo de producto.");
        }
    }

    public async Task<ApiResponse<List<ProductTypeResponseDto>>> GetAllAsync(CancellationToken cancellationToken)
    {
        try
        {
            var items = await _productTypeRepository.GetAllActiveAsync(cancellationToken);
            return ApiResponse<List<ProductTypeResponseDto>>.SuccessResponse(items.Select(MapToDto).ToList(), $"Se encontraron {items.Count} tipos de producto");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener tipos de producto");
            return ApiResponse<List<ProductTypeResponseDto>>.ErrorResponse("Error al obtener los tipos de producto.");
        }
    }

    public async Task<ApiResponse<ProductTypeResponseDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var entity = await _productTypeRepository.GetByIdAsync(id, cancellationToken);
            if (entity == null || !entity.IsActive)
                return ApiResponse<ProductTypeResponseDto>.ErrorResponse("Tipo de producto no encontrado.");

            return ApiResponse<ProductTypeResponseDto>.SuccessResponse(MapToDto(entity), "Tipo de producto encontrado.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener tipo de producto: {Id}", id);
            return ApiResponse<ProductTypeResponseDto>.ErrorResponse("Error al obtener el tipo de producto.");
        }
    }

    public async Task<ApiResponse<ProductTypeResponseDto>> UpdateAsync(Guid id, CreateProductTypeRequestDto request, CancellationToken cancellationToken)
    {
        try
        {
            var entity = await _productTypeRepository.GetByIdAsync(id, cancellationToken);
            if (entity == null || !entity.IsActive)
                return ApiResponse<ProductTypeResponseDto>.ErrorResponse("Tipo de producto no encontrado.");

            var code = request.Code.Trim().ToUpper();
            if (!string.Equals(entity.Code, code, StringComparison.OrdinalIgnoreCase))
            {
                if (await _productTypeRepository.ExistsByCodeAsync(code, cancellationToken))
                    return ApiResponse<ProductTypeResponseDto>.ErrorResponse($"Ya existe un tipo de producto con el código '{code}'.");
            }

            entity.Name = request.Name.Trim();
            entity.Code = code;
            entity.Description = request.Description?.Trim() ?? string.Empty;
            entity.Icon = request.Icon?.Trim() ?? string.Empty;

            var updated = await _productTypeRepository.UpdateAsync(entity, cancellationToken);
            return ApiResponse<ProductTypeResponseDto>.SuccessResponse(MapToDto(updated), "Tipo de producto actualizado exitosamente.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar tipo de producto: {Id}", id);
            return ApiResponse<ProductTypeResponseDto>.ErrorResponse("Error al actualizar el tipo de producto.");
        }
    }

    public async Task<ApiResponse<ProductTypeResponseDto>> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var entity = await _productTypeRepository.GetByIdAsync(id, cancellationToken);
            if (entity == null || !entity.IsActive)
                return ApiResponse<ProductTypeResponseDto>.ErrorResponse("Tipo de producto no encontrado.");

            var deleted = await _productTypeRepository.DeleteAsync(entity, cancellationToken);
            return ApiResponse<ProductTypeResponseDto>.SuccessResponse(MapToDto(deleted), "Tipo de producto eliminado exitosamente.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar tipo de producto: {Id}", id);
            return ApiResponse<ProductTypeResponseDto>.ErrorResponse("Error al eliminar el tipo de producto.");
        }
    }

    private static ProductTypeResponseDto MapToDto(ProductType pt) => new()
    {
        Id = pt.Id, Name = pt.Name, Code = pt.Code, Description = pt.Description,
        Icon = pt.Icon, IsActive = pt.IsActive, CreatedAt = pt.CreatedAt
    };
}
