using GoldenGems.Application.Common;

namespace GoldenGems.Application.Interfaces.Auth;

public interface IProfileCompletionService : IBaseService
{
    Task<bool> IsProfileCompleteAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<ApiResponse<T>> ValidateProfileOrError<T>(Guid userId, CancellationToken cancellationToken = default);
}
