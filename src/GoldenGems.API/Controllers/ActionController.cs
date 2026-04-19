using GoldenGems.Application.DTOs.Admin;
using GoldenGems.Application.Interfaces.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GoldenGems.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class ActionController : ControllerBase
{
    private readonly IActionService _actionService;

    public ActionController(IActionService actionService)
    {
        _actionService = actionService ?? throw new ArgumentNullException(nameof(actionService));
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateAction([FromBody] CreateActionRequestDto request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _actionService.CreateActionAsync(request, cancellationToken);

        if (!result.Success)
            return BadRequest(result);

        return CreatedAtAction(nameof(GetAllActions), result);
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAllActions(CancellationToken cancellationToken)
    {
        var result = await _actionService.GetAllActionsAsync(cancellationToken);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateAction(Guid id, [FromBody] CreateActionRequestDto request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var result = await _actionService.UpdateActionAsync(id, request, cancellationToken);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAction(Guid id, CancellationToken cancellationToken)
    {
        var result = await _actionService.DeleteActionAsync(id, cancellationToken);
        return result.Success ? Ok(result) : NotFound(result);
    }
}
