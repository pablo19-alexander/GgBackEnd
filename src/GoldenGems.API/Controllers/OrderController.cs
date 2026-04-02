using System.Security.Claims;
using GoldenGems.Application.DTOs.Payment;
using GoldenGems.Application.Interfaces.Payment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GoldenGems.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrderController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    private Guid GetUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CreateOrderFromConversationDto request, CancellationToken cancellationToken)
    {
        var result = await _orderService.CreateFromConversationAsync(GetUserId(), request, cancellationToken);
        if (!result.Success) return BadRequest(result);
        return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _orderService.GetByIdAsync(id, GetUserId(), cancellationToken);
        if (!result.Success) return NotFound(result);
        return Ok(result);
    }

    [HttpGet("my-orders")]
    public async Task<IActionResult> GetMyOrders(CancellationToken cancellationToken)
    {
        var result = await _orderService.GetMyOrdersAsync(GetUserId(), cancellationToken);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpPut("{id:guid}/status")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateOrderStatusDto request, CancellationToken cancellationToken)
    {
        var result = await _orderService.UpdateStatusAsync(id, request.Status, cancellationToken);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpPost("{id:guid}/cancel")]
    public async Task<IActionResult> Cancel(Guid id, CancellationToken cancellationToken)
    {
        var result = await _orderService.CancelAsync(id, GetUserId(), cancellationToken);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }
}

public class UpdateOrderStatusDto
{
    public string Status { get; set; } = string.Empty;
}
