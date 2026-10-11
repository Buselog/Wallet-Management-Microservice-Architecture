namespace Document.Application.Dtos
{
    public class WalletDto
    {
        public int Id { get; set; }
        public string CustomerNo { get; set; } = string.Empty;
        public string IBAN { get; set; } = string.Empty;
        public decimal Balance { get; set; }
        public string Currency { get; set; } = "TRY";
        public bool IsActive { get; set; }
    }
}