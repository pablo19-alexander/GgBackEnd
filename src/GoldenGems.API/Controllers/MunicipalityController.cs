using GoldenGems.Application.Interfaces.Admin;
using Microsoft.AspNetCore.Mvc;

namespace GoldenGems.API.Controllers;

[ApiController]
[Route("api/municipality")]
public class MunicipalityController : ControllerBase
{
    private readonly IMunicipalityService _municipalityService;

    public MunicipalityController(IMunicipalityService municipalityService)
    {
        _municipalityService = municipalityService ?? throw new ArgumentNullException(nameof(municipalityService));
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _municipalityService.GetAllAsync(cancellationToken);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _municipalityService.GetByIdAsync(id, cancellationToken);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpGet("by-department/{departmentId:guid}")]
    public async Task<IActionResult> GetByDepartmentId(Guid departmentId, CancellationToken cancellationToken)
    {
        var result = await _municipalityService.GetByDepartmentIdAsync(departmentId, cancellationToken);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}
