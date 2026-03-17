using GoldenGems.Application.DTOs.Admin;
using GoldenGems.Application.Interfaces.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GoldenGems.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class RoleController : ControllerBase
{
    private readonly IRoleService _roleService;

    public RoleController(IRoleService roleService)
    {
        _roleService = roleService ?? throw new ArgumentNullException(nameof(roleService));
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateRole([FromBody] CreateRoleRequestDto request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _roleService.CreateRoleAsync(request, cancellationToken);

        if (!result.Success)
            return BadRequest(result);

        return CreatedAtAction(nameof(GetAllRoles), result);
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAllRoles(CancellationToken cancellationToken)
    {
        var result = await _roleService.GetAllRolesAsync(cancellationToken);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }
}
