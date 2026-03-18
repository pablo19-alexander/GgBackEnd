using GoldenGems.Application.Interfaces.Business;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GoldenGems.API.Controllers;

[ApiController]
[Route("api/product/{productId:guid}/images")]
[Authorize]
public class ProductImageController : ControllerBase
{
    private readonly IProductImageService _imageService;

    public ProductImageController(IProductImageService imageService)
    {
        _imageService = imageService;
    }

    [HttpPost]
    public async Task<IActionResult> Upload(Guid productId, IFormFile file, CancellationToken cancellationToken)
    {
        var result = await _imageService.UploadAsync(productId, file, cancellationToken);
        if (!result.Success) return BadRequest(result);

        return Created($"api/product/{productId}/images", result);
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> GetByProduct(Guid productId, CancellationToken cancellationToken)
    {
        var result = await _imageService.GetByProductIdAsync(productId, cancellationToken);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPut("{imageId:guid}/primary")]
    public async Task<IActionResult> SetPrimary(Guid productId, Guid imageId, CancellationToken cancellationToken)
    {
        var result = await _imageService.SetPrimaryAsync(productId, imageId, cancellationToken);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("{imageId:guid}")]
    public async Task<IActionResult> Delete(Guid productId, Guid imageId, CancellationToken cancellationToken)
    {
        var result = await _imageService.DeleteAsync(productId, imageId, cancellationToken);
        return result.Success ? Ok(result) : NotFound(result);
    }
}
