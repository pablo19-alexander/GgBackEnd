using GoldenGems.Application.DTOs.Admin;
using GoldenGems.Application.Interfaces.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GoldenGems.API.Controllers;

[ApiController]
[Route("api/admin/users")]
[Authorize(Roles = "Admin")]
public class AdminUserController : ControllerBase
{
    private readonly IAdminUserService _adminUserService;

    public AdminUserController(IAdminUserService adminUserService)
    {
        _adminUserService = adminUserService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _adminUserService.GetAllUsersAsync(cancellationToken);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("without-person")]
    public async Task<IActionResult> GetUsersWithoutPerson(CancellationToken cancellationToken)
    {
        var result = await _adminUserService.GetUsersWithoutPersonAsync(cancellationToken);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPut("{userId:guid}/password")]
    public async Task<IActionResult> ChangePassword(Guid userId, [FromBody] AdminChangePasswordRequestDto request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _adminUserService.ChangePasswordAsync(userId, request, cancellationToken);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPut("{userId:guid}/roles")]
    public async Task<IActionResult> UpdateRoles(Guid userId, [FromBody] AdminUpdateRolesRequestDto request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _adminUserService.UpdateRolesAsync(userId, request, cancellationToken);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}
