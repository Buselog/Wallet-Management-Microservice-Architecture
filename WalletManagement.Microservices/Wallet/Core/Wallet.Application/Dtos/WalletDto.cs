using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public DateTime CreatedDate { get; set; }
        public List<WalletTransactionDto> Transactions { get; set; } = new();
    }
}
