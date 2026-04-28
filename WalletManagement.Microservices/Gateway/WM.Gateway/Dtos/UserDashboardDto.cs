namespace WM.Gateway.Dtos
{
    public class UserDashboardDto
    {
        public string FullName { get; set; }
        public List<WalletDetailDto> Wallets { get; set; }
        public List<CurrencySummaryDto> CurrencySummaries { get; set; }
    }

    public class CurrencySummaryDto
    {
        public string Currency { get; set; }
        public decimal TotalBalance { get; set; }
    }
}
