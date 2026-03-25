namespace Wallet.Application.Dtos
{
    public class ExchangeRateDto
    {
        public string CurrencyCode { get; set; } 
        public decimal BuyingRate { get; set; }
        public decimal SellingRate { get; set; }
        public DateTime Date { get; set; }
    }
}
