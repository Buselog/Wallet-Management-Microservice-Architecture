
namespace Wallet.Application.Dtos
{
    public class CreateTransactionDto
    {
        public int WalletId { get; set; }
        public decimal Amount { get; set; }
        public string ReferenceId { get; set; } = string.Empty;
        public string? TargetAddress { get; set; }
    }
}
