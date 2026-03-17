using GoldenGems.Application.Common;
using GoldenGems.Application.DTOs.Admin;

namespace GoldenGems.Application.Interfaces.Admin;

public interface IDocumentTypeService : IBaseService
{
    Task<ApiResponse<DocumentTypeResponseDto>> CreateAsync(CreateDocumentTypeRequestDto request, CancellationToken cancellationToken);
    Task<ApiResponse<List<DocumentTypeResponseDto>>> GetAllAsync(CancellationToken cancellationToken);
    Task<ApiResponse<DocumentTypeResponseDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<ApiResponse<DocumentTypeResponseDto>> UpdateAsync(Guid id, CreateDocumentTypeRequestDto request, CancellationToken cancellationToken);
    Task<ApiResponse<DocumentTypeResponseDto>> DeleteAsync(Guid id, CancellationToken cancellationToken);
}
