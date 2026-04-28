namespace WM.Gateway.Dtos
{
    public class WalletDetailDto
    {
        public int Id { get; set; }
        public string Currency { get; set; }
        public decimal Balance { get; set; }
        public int Type { get; set; }
    }
}
