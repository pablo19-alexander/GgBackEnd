using GoldenGems.Application.Common;
using GoldenGems.Application.DTOs.Business;
using Microsoft.AspNetCore.Http;

namespace GoldenGems.Application.Interfaces.Business;

public interface IProductImageService : IBaseService
{
    Task<ApiResponse<ProductImageDto>> UploadAsync(Guid productId, IFormFile file, CancellationToken cancellationToken);
    Task<ApiResponse<List<ProductImageDto>>> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken);
    Task<ApiResponse<ProductImageDto>> SetPrimaryAsync(Guid productId, Guid imageId, CancellationToken cancellationToken);
    Task<ApiResponse<ProductImageDto>> DeleteAsync(Guid productId, Guid imageId, CancellationToken cancellationToken);
}
