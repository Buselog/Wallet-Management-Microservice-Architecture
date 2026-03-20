using Ardalis.GuardClauses;
using Wallet.Domain.Entities.Abstract;

namespace Wallet.Domain.Entities.Concretes
{
    public class WalletTransaction : BaseEntity
    {
        public int WalletId { get; private set; }
        public decimal Amount { get; private set; }
        public string TransactionType { get; private set; } = string.Empty;
        public string TargetAddress { get; private set; } = string.Empty;
        public string Description { get; private set; } = string.Empty;
        public string ReferenceId { get; private set; } = string.Empty; 
        public Wallet Wallet { get; set; } = null!;

        private WalletTransaction() { 
        
        }

        public WalletTransaction(int walletId, decimal amount, string referenceId, string transactionType)
        {
            Guard.Against.Zero(walletId, nameof(walletId), "Geçerli bir cüzdan ID olmalı.");
            Guard.Against.Zero(amount, nameof(amount), "İşlem tutarı sıfır olamaz.");
            Guard.Against.NullOrWhiteSpace(referenceId, nameof(referenceId));
            Guard.Against.NullOrWhiteSpace(transactionType, nameof(transactionType), "İşlem tipi boş olamaz.");

            WalletId = walletId;
            Amount = amount;
            ReferenceId = referenceId;
            TransactionType = transactionType;
        }
    }
}
