using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wallet.Domain.Entities.Abstract;

namespace Wallet.Domain.Entities.Concretes
{
    public class WalletTransaction : BaseEntity
    {
        public int WalletId { get; set; }
        public decimal Amount { get; set; }
        public string TransactionType { get; set; } = string.Empty;
        public string TargetAddress { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ReferenceId { get; set; } = string.Empty; 
        public Wallet Wallet { get; set; } = null!;
    }
}
