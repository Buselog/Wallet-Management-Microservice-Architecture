using Microsoft.AspNetCore.Http;

namespace Document.Application.Dtos
{
    public class InvoiceRequestDto
    {
        public IFormFile File { get; set; }

        public int WalletId { get; set; }
    }
}

