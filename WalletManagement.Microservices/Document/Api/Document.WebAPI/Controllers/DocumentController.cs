using Document.WebAPI.Dtos;
using Document.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Document.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DocumentController : ControllerBase
{
    private readonly IDocumentParserService _documentParserService;

    public DocumentController(IDocumentParserService documentParserService)
    {
        _documentParserService = documentParserService;
    }

    [HttpPost("process")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> ProcessInvoice([FromForm] InvoiceRequestDto request)
    {
        if (request.File == null)
        {
            var result = await _documentParserService.ParseInvoiceAsync(null!, string.Empty, request.WalletId);
            return Ok(result);
        }

        await using var stream = request.File.OpenReadStream();
        var parseResult = await _documentParserService.ParseInvoiceAsync(stream, request.File.FileName, request.WalletId);

        return Ok(parseResult);
    }
}