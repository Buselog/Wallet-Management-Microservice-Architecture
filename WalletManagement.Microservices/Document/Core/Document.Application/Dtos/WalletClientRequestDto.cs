namespace Document.Application.Dtos;

public class WalletTransactionRequestDto
{
    public int? WalletId { get; set; }
    public decimal? Amount { get; set; }
    public string? ReferenceId { get; set; }
}
