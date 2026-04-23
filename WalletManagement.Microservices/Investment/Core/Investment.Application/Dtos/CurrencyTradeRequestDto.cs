namespace Investment.Application.Dtos
{
    public class CurrencyTradeRequestDto
    {
        public string CustomerNo { get; set; } 
        public int SourceWalletId { get; set; } 
        public int TargetWalletId { get; set; }
        public decimal Amount { get; set; }
        public decimal TargetRate { get; set; }
        public string TradeType { get; set; }
        public string ReferenceId { get; set; }
    }
}
