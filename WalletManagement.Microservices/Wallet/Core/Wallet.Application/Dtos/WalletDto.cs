using Wallet.Domain.Entities.Enums;

namespace Wallet.Application.Dtos
{
    public class WalletDto
    {
        public int Id { get; set; }
        public string CustomerNo { get; set; } = string.Empty; 
        public string IBAN { get; set; } = string.Empty; 
        public decimal Balance { get; set; }
        public string Currency { get; set; } = "TRY";
        public WalletType Type { get; set; }
        public string TypeName => Type switch
        {
            WalletType.Saving => "Vadeli Hesap",
            WalletType.Checking => "Vadesiz Hesap",
            WalletType.Investment => "Yatırım Hesabı",
            _ => "Tanımlanmamış Cüzdan Tipi"
        };
        public DateTime CreatedDate { get; set; }
        public List<WalletTransactionDto> Transactions { get; set; } = new();
    }
}
