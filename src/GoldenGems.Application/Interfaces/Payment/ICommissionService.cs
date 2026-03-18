using GoldenGems.Application.Common;
using GoldenGems.Application.DTOs.Payment;

namespace GoldenGems.Application.Interfaces.Payment;

public interface ICommissionService : IBaseService
{
    Task<ApiResponse<CommissionResponseDto>> CreateAsync(CommissionRequestDto request, CancellationToken cancellationToken);
    Task<ApiResponse<List<CommissionResponseDto>>> GetAllAsync(CancellationToken cancellationToken);
    Task<ApiResponse<CommissionResponseDto>> GetByCompanyIdAsync(Guid companyId, CancellationToken cancellationToken);
    Task<ApiResponse<CommissionResponseDto>> UpdateAsync(Guid id, CommissionRequestDto request, CancellationToken cancellationToken);
}
