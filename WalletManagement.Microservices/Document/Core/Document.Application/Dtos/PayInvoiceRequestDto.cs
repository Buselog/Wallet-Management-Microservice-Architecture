namespace Document.Application.Dtos
{
    public class PayInvoiceRequestDto
    {
        public int? WalletId { get; set; }
        public decimal? Amount { get; set; }
        public string ReferenceId { get; set; } = string.Empty; 
    }
}
