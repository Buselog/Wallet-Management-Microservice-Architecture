
namespace Wallet.Application.Dtos
{
    public class PayInvoiceRequestDto
    {
        public int? WalletId { get; set; }
        public decimal? Amount { get; set; }
        public string? Currency { get; set; }
        public string? ReferenceId { get; set; }
        public string? BillerName { get; set; }
    }
}
