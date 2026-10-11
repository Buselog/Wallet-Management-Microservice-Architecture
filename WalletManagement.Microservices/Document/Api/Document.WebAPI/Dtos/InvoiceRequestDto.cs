namespace Document.WebAPI.Dtos
{
    public class InvoiceRequestDto
    {
        public IFormFile? File { get; set; }

        public int WalletId { get; set; }
    }
}

