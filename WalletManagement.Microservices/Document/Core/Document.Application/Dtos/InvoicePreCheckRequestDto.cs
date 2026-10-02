namespace Document.Application.Dtos
{
    public class InvoicePreCheckRequestDto
    {
        public int WalletId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public string ReferenceId { get; set; } = string.Empty;
    }
}
