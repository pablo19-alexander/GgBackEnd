using GoldenGems.Application.Common;
using GoldenGems.Application.DTOs.Business;
using Microsoft.AspNetCore.Http;

namespace GoldenGems.Application.Interfaces.Business;

public interface ICompanyService : IBaseService
{
    Task<ApiResponse<CompanyResponseDto>> RegisterAsync(CreateCompanyRequestDto request, Guid ownerId, CancellationToken cancellationToken);
    Task<ApiResponse<CompanyResponseDto>> AdminRegisterAsync(AdminCreateCompanyRequestDto request, CancellationToken cancellationToken);
    Task<ApiResponse<List<CompanyResponseDto>>> GetAllAsync(CancellationToken cancellationToken);
    Task<ApiResponse<CompanyResponseDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<ApiResponse<CompanyResponseDto>> UpdateAsync(Guid id, CreateCompanyRequestDto request, Guid requesterId, CancellationToken cancellationToken);
    Task<ApiResponse<CompanyResponseDto>> DeleteAsync(Guid id, CancellationToken cancellationToken);
    Task<ApiResponse<CompanyResponseDto>> UploadLogoAsync(Guid id, IFormFile file, CancellationToken cancellationToken);
}
