using Document.Application.Dtos;
using Document.Application.Services;
using Document.WebAPI.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Document.WebAPI.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class DocumentController : ControllerBase
{
    private readonly IDocumentParserService _documentParserService;

    public DocumentController(IDocumentParserService documentParserService)
    {
        _documentParserService = documentParserService;
    }

    [HttpPost("preview")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Preview(IFormFile file, int walletId, CancellationToken cancellationToken)
    {
        using var stream = file.OpenReadStream();
        var result = await _documentParserService.ExtractAndPreviewAsync(stream, file.FileName, walletId, cancellationToken);
        return Ok(result);
    }

    [HttpPost("confirm")]
    public async Task<IActionResult> Confirm([FromBody] ConfirmInvoicePaymentRequestDto request)
    {
        var result = await _documentParserService.ConfirmAndPayAsync(request);
        return Ok(result);
    }
}