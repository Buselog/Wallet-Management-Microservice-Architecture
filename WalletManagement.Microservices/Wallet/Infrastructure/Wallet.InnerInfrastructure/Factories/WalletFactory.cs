using Wallet.Application.Factories;
using Wallet.Application.Helpers;
using Wallet.Domain.Entities.Enums;
using WalletEntity = Wallet.Domain.Entities.Concretes.Wallet;
namespace Wallet.InnerInfrastructure.Factories
{
    public class WalletFactory : IWalletFactory
    {
        private const string BankCode = "9999"; 
        private const string ReserveDigit = "0";
        public WalletEntity CreateWallet(string customerNo, string currency, WalletType type)
        {
            string bban = BankCode + ReserveDigit + customerNo.PadLeft(16, '0');

            string checkDigits = IbanHelper.CalculateCheckDigits(bban);

            return new WalletEntity
            {
                CustomerNo = customerNo,
                Currency = currency,
                Type = type,
                IBAN = $"TR{checkDigits}{bban}",
                Balance = 0,
                IsActive = true,
            };

        }

    }
}
