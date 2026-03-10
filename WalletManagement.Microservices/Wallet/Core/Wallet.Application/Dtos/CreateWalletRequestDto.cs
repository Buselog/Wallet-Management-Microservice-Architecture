using Wallet.Domain.Entities.Enums;

namespace Wallet.Application.Dtos
{
    public class CreateWalletRequestDto
    {
       public string CustomerNo { get; set; }
       public string Currency { get; set; }
       public WalletType Type { get; set; }
    }
}
