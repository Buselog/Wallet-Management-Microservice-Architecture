namespace Wallet.Application.Dtos
{
    public class InvoicePreCheckResponseDto
    {
        public bool CanBePaid { get; set; }
        public bool IsAlreadyPaid { get; set; }
        public bool IsCurrencyMatched { get; set; }
        public bool HasSufficientBalance { get; set; }
        public string? FailureReason { get; set; }
    }
}
