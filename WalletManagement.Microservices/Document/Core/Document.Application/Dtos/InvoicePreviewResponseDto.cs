namespace Document.Application.Dtos
{
    public class InvoicePreviewResponseDto
    {
        public string? InvoiceNumber { get; set; }
        public string? BillerName { get; set; }
        public decimal? TotalAmount { get; set; }
        public string? Currency { get; set; }
        public DateOnly IssueDate { get; set; }
        public bool CanBePaid { get; set; }
        public bool IsAlreadyPaid { get; set; }
        public bool IsCurrencyMatched { get; set; }
        public bool HasSufficientBalance { get; set; }
        public string? ValidationMessage { get; set; }
    }
}
