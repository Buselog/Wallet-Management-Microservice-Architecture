namespace WM.Gateway.Dtos
{
    public class UserDashboardDto
    {
        public string FullName { get; set; }
        public List<WalletDetailDto> Wallets { get; set; }
        public decimal TotalBalanceTRY { get; set; }
    }
}
