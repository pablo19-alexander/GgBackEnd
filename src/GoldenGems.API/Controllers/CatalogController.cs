using GoldenGems.Application.Interfaces.Business;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GoldenGems.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public class CatalogController : ControllerBase
{
    private readonly IProductService _productService;

    public CatalogController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    public async Task<IActionResult> GetCatalog(
        [FromQuery] Guid? companyId,
        [FromQuery] Guid? productTypeId,
        [FromQuery] decimal? minPrice,
        [FromQuery] decimal? maxPrice,
        [FromQuery] string? search,
        [FromQuery] string? sortBy,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var result = await _productService.GetCatalogAsync(
            companyId, productTypeId, minPrice, maxPrice, search, sortBy, page, pageSize, cancellationToken);

        return result.Success ? Ok(result) : BadRequest(result);
    }
}
