using GoldenGems.Application.Common;
using GoldenGems.Application.DTOs.Business;
using GoldenGems.Application.Interfaces.Business;
using GoldenGems.Domain.Entities.Business;
using GoldenGems.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace GoldenGems.Application.Services.Business;

public class ProductService : BaseService, IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly ICompanyRepository _companyRepository;
    private readonly IProductTypeRepository _productTypeRepository;

    public ProductService(
        IProductRepository productRepository,
        ICompanyRepository companyRepository,
        IProductTypeRepository productTypeRepository,
        ILogger<ProductService> logger) : base(logger)
    {
        _productRepository = productRepository;
        _companyRepository = companyRepository;
        _productTypeRepository = productTypeRepository;
    }

    public async Task<ApiResponse<ProductResponseDto>> CreateAsync(CreateProductRequestDto request, CancellationToken cancellationToken)
    {
        try
        {
            if (!await _companyRepository.ExistsAsync(request.CompanyId, cancellationToken))
                return ApiResponse<ProductResponseDto>.ErrorResponse("La empresa no existe.");

            if (!await _productTypeRepository.ExistsAsync(request.ProductTypeId, cancellationToken))
                return ApiResponse<ProductResponseDto>.ErrorResponse("El tipo de producto no existe.");

            var product = new Product
            {
                Name = request.Name.Trim(),
                Description = request.Description?.Trim() ?? string.Empty,
                ReferencePrice = request.ReferencePrice,
                IsNegotiable = request.IsNegotiable,
                InitialChatMessage = request.InitialChatMessage?.Trim() ?? string.Empty,
                CompanyId = request.CompanyId,
                ProductTypeId = request.ProductTypeId
            };

            var created = await _productRepository.CreateAsync(product, cancellationToken);
            var detail = await _productRepository.GetByIdWithDetailsAsync(created.Id, cancellationToken);
            return ApiResponse<ProductResponseDto>.SuccessResponse(MapToDto(detail!), "Producto creado exitosamente.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear producto");
            return ApiResponse<ProductResponseDto>.ErrorResponse("Error al crear el producto.");
        }
    }

    public async Task<ApiResponse<ProductResponseDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var product = await _productRepository.GetByIdWithDetailsAsync(id, cancellationToken);
            if (product == null)
                return ApiResponse<ProductResponseDto>.ErrorResponse("Producto no encontrado.");

            return ApiResponse<ProductResponseDto>.SuccessResponse(MapToDto(product), "Producto encontrado.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener producto: {Id}", id);
            return ApiResponse<ProductResponseDto>.ErrorResponse("Error al obtener el producto.");
        }
    }

    public async Task<ApiResponse<List<ProductResponseDto>>> GetByCompanyIdAsync(Guid companyId, CancellationToken cancellationToken)
    {
        try
        {
            var products = await _productRepository.GetByCompanyIdAsync(companyId, cancellationToken);
            return ApiResponse<List<ProductResponseDto>>.SuccessResponse(products.Select(MapToDto).ToList(), $"Se encontraron {products.Count} productos");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener productos por empresa: {CompanyId}", companyId);
            return ApiResponse<List<ProductResponseDto>>.ErrorResponse("Error al obtener los productos.");
        }
    }

    public async Task<ApiResponse<List<ProductResponseDto>>> GetByProductTypeIdAsync(Guid productTypeId, CancellationToken cancellationToken)
    {
        try
        {
            var products = await _productRepository.GetByProductTypeIdAsync(productTypeId, cancellationToken);
            return ApiResponse<List<ProductResponseDto>>.SuccessResponse(products.Select(MapToDto).ToList(), $"Se encontraron {products.Count} productos");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener productos por tipo: {ProductTypeId}", productTypeId);
            return ApiResponse<List<ProductResponseDto>>.ErrorResponse("Error al obtener los productos.");
        }
    }

    public async Task<ApiResponse<List<ProductResponseDto>>> SearchAsync(string query, CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(query))
                return ApiResponse<List<ProductResponseDto>>.ErrorResponse("El término de búsqueda es requerido.");

            var products = await _productRepository.SearchByNameAsync(query, cancellationToken);
            return ApiResponse<List<ProductResponseDto>>.SuccessResponse(products.Select(MapToDto).ToList(), $"Se encontraron {products.Count} productos");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al buscar productos: {Query}", query);
            return ApiResponse<List<ProductResponseDto>>.ErrorResponse("Error al buscar productos.");
        }
    }

    public async Task<ApiResponse<ProductResponseDto>> UpdateAsync(Guid id, CreateProductRequestDto request, CancellationToken cancellationToken)
    {
        try
        {
            var product = await _productRepository.GetByIdAsync(id, cancellationToken);
            if (product == null || !product.IsActive)
                return ApiResponse<ProductResponseDto>.ErrorResponse("Producto no encontrado.");

            product.Name = request.Name.Trim();
            product.Description = request.Description?.Trim() ?? string.Empty;
            product.ReferencePrice = request.ReferencePrice;
            product.IsNegotiable = request.IsNegotiable;
            product.InitialChatMessage = request.InitialChatMessage?.Trim() ?? string.Empty;
            product.CompanyId = request.CompanyId;
            product.ProductTypeId = request.ProductTypeId;

            await _productRepository.UpdateAsync(product, cancellationToken);
            var detail = await _productRepository.GetByIdWithDetailsAsync(id, cancellationToken);
            return ApiResponse<ProductResponseDto>.SuccessResponse(MapToDto(detail!), "Producto actualizado exitosamente.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar producto: {Id}", id);
            return ApiResponse<ProductResponseDto>.ErrorResponse("Error al actualizar el producto.");
        }
    }

    public async Task<ApiResponse<ProductResponseDto>> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var product = await _productRepository.GetByIdAsync(id, cancellationToken);
            if (product == null || !product.IsActive)
                return ApiResponse<ProductResponseDto>.ErrorResponse("Producto no encontrado.");

            var deleted = await _productRepository.DeleteAsync(product, cancellationToken);
            return ApiResponse<ProductResponseDto>.SuccessResponse(MapToDto(deleted), "Producto eliminado exitosamente.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar producto: {Id}", id);
            return ApiResponse<ProductResponseDto>.ErrorResponse("Error al eliminar el producto.");
        }
    }

    public async Task<ApiResponse<CatalogResponseDto>> GetCatalogAsync(
        Guid? companyId, Guid? productTypeId, decimal? minPrice, decimal? maxPrice,
        string? search, string? sortBy, int page, int pageSize,
        CancellationToken cancellationToken)
    {
        try
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 50) pageSize = 50;

            var (items, totalCount) = await _productRepository.GetCatalogAsync(
                companyId, productTypeId, minPrice, maxPrice, search, sortBy, page, pageSize, cancellationToken);

            var catalog = new CatalogResponseDto
            {
                Items = items.Select(MapToDto).ToList(),
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            };

            return ApiResponse<CatalogResponseDto>.SuccessResponse(catalog, $"Se encontraron {totalCount} productos");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener catálogo");
            return ApiResponse<CatalogResponseDto>.ErrorResponse("Error al obtener el catálogo.");
        }
    }

    private static ProductResponseDto MapToDto(Product p) => new()
    {
        Id = p.Id,
        Name = p.Name,
        Description = p.Description,
        ReferencePrice = p.ReferencePrice,
        IsNegotiable = p.IsNegotiable,
        InitialChatMessage = p.InitialChatMessage,
        CompanyId = p.CompanyId,
        CompanyName = p.Company?.Name ?? string.Empty,
        CompanyLogo = p.Company?.Logo ?? string.Empty,
        ProductTypeId = p.ProductTypeId,
        ProductTypeName = p.ProductType?.Name ?? string.Empty,
        PrimaryImageUrl = p.Images?.FirstOrDefault(i => i.IsPrimary)?.Url ?? p.Images?.FirstOrDefault()?.Url ?? string.Empty,
        Images = p.Images?.Select(i => new ProductImageDto
        {
            Id = i.Id, Url = i.Url, AltText = i.AltText, DisplayOrder = i.DisplayOrder, IsPrimary = i.IsPrimary
        }).ToList() ?? new(),
        IsActive = p.IsActive,
        CreatedAt = p.CreatedAt
    };
}
