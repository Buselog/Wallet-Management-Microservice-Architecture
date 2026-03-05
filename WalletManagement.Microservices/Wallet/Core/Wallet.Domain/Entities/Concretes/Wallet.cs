using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wallet.Domain.Entities.Abstract;

namespace Wallet.Domain.Entities.Concretes
{
    public class Wallet : BaseEntity
    {
        public string CustomerNo { get; set; } = string.Empty;
        public string IBAN { get; set; } = string.Empty; 
        public decimal Balance { get; set; }
        public string Currency { get; set; } = "TRY";
        public bool IsActive { get; set; } = true; 

        [Timestamp]
        public byte[] RowVersion { get; set; }
        public ICollection<WalletTransaction> Transactions { get; set; } = new List<WalletTransaction>();
    }
}
