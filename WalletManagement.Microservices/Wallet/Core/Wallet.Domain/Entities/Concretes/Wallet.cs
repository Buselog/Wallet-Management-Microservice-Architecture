using Ardalis.GuardClauses;
using System.ComponentModel.DataAnnotations;
using Wallet.Domain.Entities.Abstract;
using Wallet.Domain.Entities.Enums;
using Wallet.Domain.Exceptions;

namespace Wallet.Domain.Entities.Concretes
{
    public class Wallet : BaseEntity
    {
        public string CustomerNo { get; private set; } = string.Empty;
        public string IBAN { get; private set; } = string.Empty;
        public int Suffix { get; private set; }
        public decimal Balance { get; private set; }
        public string Currency { get; private set; } = "TRY";
        public WalletType Type { get; private set; } = WalletType.Checking;
        public bool IsActive { get; set; } = true;

        [Timestamp]
        public byte[] RowVersion { get; set; }
        public ICollection<WalletTransaction> Transactions { get; set; } = new List<WalletTransaction>();

        private Wallet() {

        }

        public Wallet(string customerNo, string iban, string currency, WalletType type)
        {
            Guard.Against.NullOrWhiteSpace(customerNo, nameof(customerNo));

            Guard.Against.NullOrWhiteSpace(iban, nameof(iban));

            Guard.Against.LengthOutOfRange(currency, 3, 3, nameof(currency));

            Guard.Against.EnumOutOfRange<WalletType>(type, nameof(type), "Geçersiz cüzdan tipi.");

            CustomerNo = customerNo;
            IBAN = iban;
            Currency = currency;
            Type = type;
            Balance = 0; 
            IsActive = true;
        }

        public void Withdraw(decimal amount) 
        {
            Guard.Against.NegativeOrZero(amount, nameof(amount));

            if (Balance < amount)
            {
                throw new BaseBusinessException("Yetersiz bakiye.");
            }

            Balance -= amount;
        }

        public void Deposit(decimal amount)
        {
            Guard.Against.NegativeOrZero(amount, nameof(amount));

            Balance += amount;
        }
    }
}
