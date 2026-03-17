using GoldenGems.Application.Common;
using GoldenGems.Application.DTOs.People;

namespace GoldenGems.Application.Interfaces.People;

public interface IContactService : IBaseService
{
    Task<ApiResponse<ContactResponseDto>> CreateAsync(CreateContactRequestDto request, CancellationToken cancellationToken);
    Task<ApiResponse<ContactResponseDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<ApiResponse<ContactResponseDto>> UpdateAsync(Guid id, CreateContactRequestDto request, CancellationToken cancellationToken);
    Task<ApiResponse<ContactResponseDto>> DeleteAsync(Guid id, CancellationToken cancellationToken);
}
