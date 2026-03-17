using GoldenGems.Application.DTOs.Admin;
using GoldenGems.Application.Interfaces.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GoldenGems.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class ModuleController : ControllerBase
{
    private readonly IModuleService _moduleService;

    public ModuleController(IModuleService moduleService)
    {
        _moduleService = moduleService ?? throw new ArgumentNullException(nameof(moduleService));
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateModule([FromBody] CreateModuleRequestDto request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _moduleService.CreateAsync(request, cancellationToken);

        if (!result.Success)
            return BadRequest(result);

        return CreatedAtAction(nameof(GetModuleById), new { id = result.Data!.Id }, result);
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAllModules(CancellationToken cancellationToken)
    {
        var result = await _moduleService.GetAllAsync(cancellationToken);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetModuleById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _moduleService.GetByIdAsync(id, cancellationToken);

        if (!result.Success)
            return NotFound(result);

        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateModule(Guid id, [FromBody] CreateModuleRequestDto request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _moduleService.UpdateAsync(id, request, cancellationToken);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteModule(Guid id, CancellationToken cancellationToken)
    {
        var result = await _moduleService.DeleteAsync(id, cancellationToken);

        if (!result.Success)
            return NotFound(result);

        return Ok(result);
    }
}
