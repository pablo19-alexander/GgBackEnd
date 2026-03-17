using GoldenGems.Application.DTOs.Admin;
using GoldenGems.Application.Interfaces.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GoldenGems.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class FormController : ControllerBase
{
    private readonly IFormService _formService;

    public FormController(IFormService formService)
    {
        _formService = formService ?? throw new ArgumentNullException(nameof(formService));
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateForm([FromBody] CreateFormRequestDto request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _formService.CreateAsync(request, cancellationToken);

        if (!result.Success)
            return BadRequest(result);

        return CreatedAtAction(nameof(GetFormById), new { id = result.Data!.Id }, result);
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAllForms(CancellationToken cancellationToken)
    {
        var result = await _formService.GetAllAsync(cancellationToken);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetFormById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _formService.GetByIdAsync(id, cancellationToken);

        if (!result.Success)
            return NotFound(result);

        return Ok(result);
    }

    [HttpGet("by-module/{moduleId}")]
    public async Task<IActionResult> GetFormsByModule(Guid moduleId, CancellationToken cancellationToken)
    {
        var result = await _formService.GetByModuleIdAsync(moduleId, cancellationToken);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateForm(Guid id, [FromBody] CreateFormRequestDto request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _formService.UpdateAsync(id, request, cancellationToken);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteForm(Guid id, CancellationToken cancellationToken)
    {
        var result = await _formService.DeleteAsync(id, cancellationToken);

        if (!result.Success)
            return NotFound(result);

        return Ok(result);
    }
}
