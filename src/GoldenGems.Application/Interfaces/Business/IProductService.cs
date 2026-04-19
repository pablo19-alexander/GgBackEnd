using GoldenGems.Application.Common;
using GoldenGems.Application.DTOs.Business;

namespace GoldenGems.Application.Interfaces.Business;

public interface IProductService : IBaseService
{
    Task<ApiResponse<ProductResponseDto>> CreateAsync(CreateProductRequestDto request, CancellationToken cancellationToken);
    Task<ApiResponse<ProductResponseDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<ApiResponse<List<ProductResponseDto>>> GetByCompanyIdAsync(Guid companyId, CancellationToken cancellationToken);
    Task<ApiResponse<List<ProductResponseDto>>> GetAllByCompanyIdAsync(Guid companyId, CancellationToken cancellationToken);
    Task<ApiResponse<ProductResponseDto>> ToggleStatusAsync(Guid id, CancellationToken cancellationToken);
    Task<ApiResponse<List<ProductResponseDto>>> GetByProductTypeIdAsync(Guid productTypeId, CancellationToken cancellationToken);
    Task<ApiResponse<List<ProductResponseDto>>> SearchAsync(string query, CancellationToken cancellationToken);
    Task<ApiResponse<ProductResponseDto>> UpdateAsync(Guid id, CreateProductRequestDto request, CancellationToken cancellationToken);
    Task<ApiResponse<ProductResponseDto>> DeleteAsync(Guid id, CancellationToken cancellationToken);
    Task<ApiResponse<CatalogResponseDto>> GetCatalogAsync(
        Guid? companyId, Guid? productTypeId, decimal? minPrice, decimal? maxPrice,
        string? search, string? sortBy, int page, int pageSize,
        CancellationToken cancellationToken);
}
