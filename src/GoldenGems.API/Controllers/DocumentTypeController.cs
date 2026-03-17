using GoldenGems.Application.DTOs.Admin;
using GoldenGems.Application.Interfaces.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GoldenGems.API.Controllers;

[ApiController]
[Route("api/document-type")]
[Authorize(Roles = "Admin")]
public class DocumentTypeController : ControllerBase
{
    private readonly IDocumentTypeService _documentTypeService;

    public DocumentTypeController(IDocumentTypeService documentTypeService)
    {
        _documentTypeService = documentTypeService ?? throw new ArgumentNullException(nameof(documentTypeService));
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CreateDocumentTypeRequestDto request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _documentTypeService.CreateAsync(request, cancellationToken);

        if (!result.Success)
            return BadRequest(result);

        return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result);
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _documentTypeService.GetAllAsync(cancellationToken);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _documentTypeService.GetByIdAsync(id, cancellationToken);

        if (!result.Success)
            return NotFound(result);

        return Ok(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] CreateDocumentTypeRequestDto request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _documentTypeService.UpdateAsync(id, request, cancellationToken);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _documentTypeService.DeleteAsync(id, cancellationToken);

        if (!result.Success)
            return NotFound(result);

        return Ok(result);
    }
}
