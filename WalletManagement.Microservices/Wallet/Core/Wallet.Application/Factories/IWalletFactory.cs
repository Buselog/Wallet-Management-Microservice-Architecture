using Wallet.Domain.Entities.Enums;
using WalletEntity = Wallet.Domain.Entities.Concretes.Wallet;

namespace Wallet.Application.Factories
{
    public interface IWalletFactory
    {
        WalletEntity CreateWallet(string customerNo, string currency, WalletType type, int suffix);
    }
}
