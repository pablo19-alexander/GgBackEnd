using GoldenGems.Application.Common;
using GoldenGems.Application.DTOs.Auth;

namespace GoldenGems.Application.Interfaces.Auth;

public interface IAuthService : IBaseService
{
    Task<ApiResponse<AuthResponseDto>> RegisterAsync(RegisterRequestDto request, CancellationToken cancellationToken = default);
    Task<ApiResponse<AuthResponseDto>> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default);
}
