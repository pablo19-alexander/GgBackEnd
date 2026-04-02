using System.Security.Claims;
using GoldenGems.Application.DTOs.Payment;
using GoldenGems.Application.Interfaces.Payment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GoldenGems.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PaymentController : ControllerBase
{
    private readonly IPaymentService _paymentService;

    public PaymentController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    private Guid GetUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpPost("initiate")]
    public async Task<IActionResult> Initiate([FromBody] InitiatePaymentDto request, CancellationToken cancellationToken)
    {
        var result = await _paymentService.InitiatePaymentAsync(GetUserId(), request, cancellationToken);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpPost("confirm")]
    public async Task<IActionResult> Confirm([FromBody] ConfirmPaymentDto request, CancellationToken cancellationToken)
    {
        var result = await _paymentService.ConfirmPaymentAsync(request, cancellationToken);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpGet("by-order/{orderId:guid}")]
    public async Task<IActionResult> GetByOrder(Guid orderId, CancellationToken cancellationToken)
    {
        var result = await _paymentService.GetByOrderIdAsync(orderId, cancellationToken);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }
}
