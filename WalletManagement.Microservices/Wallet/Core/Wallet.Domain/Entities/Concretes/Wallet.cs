using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wallet.Domain.Entities.Abstract;
using Wallet.Domain.Entities.Enums;

namespace Wallet.Domain.Entities.Concretes
{
    public class Wallet : BaseEntity
    {
        public string CustomerNo { get; set; } = string.Empty;
        public string IBAN { get; set; } = string.Empty; 
        public decimal Balance { get; set; }
        public string Currency { get; set; } = "TRY";
        public WalletType Type { get; set; } = WalletType.Checking;
        public bool IsActive { get; set; } = true;
        public int Suffix { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }
        public ICollection<WalletTransaction> Transactions { get; set; } = new List<WalletTransaction>();
    }
}
