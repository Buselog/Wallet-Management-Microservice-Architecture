
namespace Wallet.Application.Dtos
{
    public class TransferRequestDto
    {
        public int FromWalletId { get; set; } 
        public string Target { get; set; }    
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public string ReferenceId { get; set; }
    }
}

