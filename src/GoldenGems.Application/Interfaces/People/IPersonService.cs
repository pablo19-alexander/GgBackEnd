using GoldenGems.Application.Common;
using GoldenGems.Application.DTOs.People;
using Microsoft.AspNetCore.Http;

namespace GoldenGems.Application.Interfaces.People;

public interface IPersonService : IBaseService
{
    Task<ApiResponse<PersonResponseDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<ApiResponse<List<PersonResponseDto>>> GetAllAsync(CancellationToken cancellationToken);
    Task<ApiResponse<PersonResponseDto>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken);
    Task<ApiResponse<PersonResponseDto>> CreateForUserAsync(Guid userId, UpdatePersonRequestDto request, CancellationToken cancellationToken);
    Task<ApiResponse<PersonResponseDto>> UpdateAsync(Guid id, UpdatePersonRequestDto request, CancellationToken cancellationToken);
    Task<ApiResponse<PersonResponseDto>> DeleteAsync(Guid id, CancellationToken cancellationToken);
    Task<ApiResponse<PersonResponseDto>> UploadPhotoAsync(Guid id, IFormFile file, CancellationToken cancellationToken);
    Task<ApiResponse<PersonResponseDto>> DeletePhotoAsync(Guid id, CancellationToken cancellationToken);
}
