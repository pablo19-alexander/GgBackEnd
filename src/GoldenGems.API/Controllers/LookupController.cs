using GoldenGems.Application.Interfaces.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GoldenGems.API.Controllers;

[ApiController]
[Route("api/lookup")]
[Authorize]
public class LookupController : ControllerBase
{
    private readonly IDocumentTypeService _documentTypeService;

    public LookupController(IDocumentTypeService documentTypeService)
    {
        _documentTypeService = documentTypeService;
    }

    [HttpGet("document-types")]
    public async Task<IActionResult> GetDocumentTypes(CancellationToken cancellationToken)
    {
        var result = await _documentTypeService.GetAllAsync(cancellationToken);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}
