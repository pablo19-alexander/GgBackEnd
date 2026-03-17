using GoldenGems.Application.Interfaces.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GoldenGems.API.Controllers;

[ApiController]
[Route("api/region")]
public class RegionController : ControllerBase
{
    private readonly IRegionService _regionService;

    public RegionController(IRegionService regionService)
    {
        _regionService = regionService ?? throw new ArgumentNullException(nameof(regionService));
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _regionService.GetAllAsync(cancellationToken);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _regionService.GetByIdAsync(id, cancellationToken);

        if (!result.Success)
            return NotFound(result);

        return Ok(result);
    }

    [HttpGet("departments")]
    public async Task<IActionResult> GetDepartments(CancellationToken cancellationToken)
    {
        var result = await _regionService.GetDepartmentsAsync(cancellationToken);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpGet("by-department/{department}")]
    public async Task<IActionResult> GetByDepartment(string department, CancellationToken cancellationToken)
    {
        var result = await _regionService.GetByDepartmentAsync(department, cancellationToken);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }
}
