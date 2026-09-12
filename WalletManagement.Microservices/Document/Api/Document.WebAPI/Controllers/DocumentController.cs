using Document.Application.Dtos;
using Document.Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Document.WebAPI.Controllers
{
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

            var parseResult = await _documentParserService.ParseInvoiceAsync(request.File, request.WalletId);

            return Ok(parseResult);
        }
    }
}