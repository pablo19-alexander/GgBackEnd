using GoldenGems.Application.Common;
using GoldenGems.Application.DTOs.Business;
using GoldenGems.Application.Interfaces.Business;
using GoldenGems.Domain.Entities.Business;
using GoldenGems.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace GoldenGems.Application.Services.Business;

public class ProductImageService : BaseService, IProductImageService
{
    private readonly IProductImageRepository _imageRepository;
    private readonly IProductRepository _productRepository;
    private readonly IImageStorageService _storageService;

    public ProductImageService(
        IProductImageRepository imageRepository,
        IProductRepository productRepository,
        IImageStorageService storageService,
        ILogger<ProductImageService> logger) : base(logger)
    {
        _imageRepository = imageRepository;
        _productRepository = productRepository;
        _storageService = storageService;
    }

    public async Task<ApiResponse<ProductImageDto>> UploadAsync(Guid productId, IFormFile file, CancellationToken cancellationToken)
    {
        try
        {
            if (!await _productRepository.ExistsAsync(productId, cancellationToken))
                return ApiResponse<ProductImageDto>.ErrorResponse("Producto no encontrado.");

            if (file.Length == 0)
                return ApiResponse<ProductImageDto>.ErrorResponse("El archivo está vacío.");

            var allowedTypes = new[] { "image/jpeg", "image/png", "image/webp", "image/gif" };
            if (!allowedTypes.Contains(file.ContentType.ToLower()))
                return ApiResponse<ProductImageDto>.ErrorResponse("Formato de imagen no permitido. Use: JPG, PNG, WEBP o GIF.");

            var url = await _storageService.UploadAsync(file, "products", cancellationToken);

            var existingImages = await _imageRepository.GetByProductIdAsync(productId, cancellationToken);
            var isPrimary = !existingImages.Any();

            var image = new ProductImage
            {
                ProductId = productId,
                Url = url,
                AltText = Path.GetFileNameWithoutExtension(file.FileName),
                DisplayOrder = existingImages.Count + 1,
                IsPrimary = isPrimary
            };

            var created = await _imageRepository.CreateAsync(image, cancellationToken);
            return ApiResponse<ProductImageDto>.SuccessResponse(MapToDto(created), "Imagen subida exitosamente.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al subir imagen para producto: {ProductId}", productId);
            return ApiResponse<ProductImageDto>.ErrorResponse("Error al subir la imagen.");
        }
    }

    public async Task<ApiResponse<List<ProductImageDto>>> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken)
    {
        try
        {
            var images = await _imageRepository.GetByProductIdAsync(productId, cancellationToken);
            return ApiResponse<List<ProductImageDto>>.SuccessResponse(images.Select(MapToDto).ToList(), $"Se encontraron {images.Count} imágenes");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener imágenes: {ProductId}", productId);
            return ApiResponse<List<ProductImageDto>>.ErrorResponse("Error al obtener las imágenes.");
        }
    }

    public async Task<ApiResponse<ProductImageDto>> SetPrimaryAsync(Guid productId, Guid imageId, CancellationToken cancellationToken)
    {
        try
        {
            var image = await _imageRepository.GetByIdAsync(imageId, cancellationToken);
            if (image == null || !image.IsActive || image.ProductId != productId)
                return ApiResponse<ProductImageDto>.ErrorResponse("Imagen no encontrada.");

            await _imageRepository.ClearPrimaryAsync(productId, cancellationToken);

            image.IsPrimary = true;
            var updated = await _imageRepository.UpdateAsync(image, cancellationToken);
            return ApiResponse<ProductImageDto>.SuccessResponse(MapToDto(updated), "Imagen marcada como principal.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al establecer imagen principal: {ImageId}", imageId);
            return ApiResponse<ProductImageDto>.ErrorResponse("Error al establecer imagen principal.");
        }
    }

    public async Task<ApiResponse<ProductImageDto>> DeleteAsync(Guid productId, Guid imageId, CancellationToken cancellationToken)
    {
        try
        {
            var image = await _imageRepository.GetByIdAsync(imageId, cancellationToken);
            if (image == null || !image.IsActive || image.ProductId != productId)
                return ApiResponse<ProductImageDto>.ErrorResponse("Imagen no encontrada.");

            await _storageService.DeleteAsync(image.Url, cancellationToken);
            var deleted = await _imageRepository.DeleteAsync(image, cancellationToken);
            return ApiResponse<ProductImageDto>.SuccessResponse(MapToDto(deleted), "Imagen eliminada exitosamente.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar imagen: {ImageId}", imageId);
            return ApiResponse<ProductImageDto>.ErrorResponse("Error al eliminar la imagen.");
        }
    }

    private static ProductImageDto MapToDto(ProductImage i) => new()
    {
        Id = i.Id, Url = i.Url, AltText = i.AltText, DisplayOrder = i.DisplayOrder, IsPrimary = i.IsPrimary
    };
}
