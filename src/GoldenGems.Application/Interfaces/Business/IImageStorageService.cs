using Microsoft.AspNetCore.Http;

namespace GoldenGems.Application.Interfaces.Business;

public interface IImageStorageService
{
    Task<string> UploadAsync(IFormFile file, string folder, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(string fileUrl, CancellationToken cancellationToken = default);
}
