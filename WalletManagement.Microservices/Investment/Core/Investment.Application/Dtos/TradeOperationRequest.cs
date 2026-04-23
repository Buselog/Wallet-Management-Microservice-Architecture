namespace Investment.Application.Dtos
{
    public class TradeOperationRequest
    {
        public int SourceWalletId { get; set; }
        public int TargetWalletId { get; set; }
        public string CurrencyCode { get; set; } 
        public decimal Amount { get; set; }
    }
}
