
namespace Wallet.Application.Dtos
{
    public class WalletTransactionDto
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public string TransactionType { get; set; } = string.Empty; 
        public string TargetAddress { get; set; } = string.Empty; 
        public string ReferenceId { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
    }
}
