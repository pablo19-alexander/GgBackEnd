using GoldenGems.Application.Common;
using GoldenGems.Application.DTOs.Business;

namespace GoldenGems.Application.Interfaces.Business;

public interface IProductTypeService : IBaseService
{
    Task<ApiResponse<ProductTypeResponseDto>> CreateAsync(CreateProductTypeRequestDto request, CancellationToken cancellationToken);
    Task<ApiResponse<List<ProductTypeResponseDto>>> GetAllAsync(CancellationToken cancellationToken);
    Task<ApiResponse<ProductTypeResponseDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<ApiResponse<ProductTypeResponseDto>> UpdateAsync(Guid id, CreateProductTypeRequestDto request, CancellationToken cancellationToken);
    Task<ApiResponse<ProductTypeResponseDto>> DeleteAsync(Guid id, CancellationToken cancellationToken);
}
