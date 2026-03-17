using GoldenGems.Application.Common;
using GoldenGems.Application.DTOs.People;

namespace GoldenGems.Application.Interfaces.People;

public interface IPersonService : IBaseService
{
    Task<ApiResponse<PersonResponseDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<ApiResponse<List<PersonResponseDto>>> GetAllAsync(CancellationToken cancellationToken);
    Task<ApiResponse<PersonResponseDto>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken);
    Task<ApiResponse<PersonResponseDto>> UpdateAsync(Guid id, UpdatePersonRequestDto request, CancellationToken cancellationToken);
    Task<ApiResponse<PersonResponseDto>> DeleteAsync(Guid id, CancellationToken cancellationToken);
}
