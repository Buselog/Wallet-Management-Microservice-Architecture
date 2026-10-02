namespace Document.Application.Dtos
{
    public class InvoiceExtractionResultDto
    {
        public bool IsReceiptOrInvoice { get; set; }
        public string? BillerName { get; set; }
        public string? InvoiceNumber { get; set; }
        public decimal? TotalAmount { get; set; }
        public string? Currency { get; set; }
        public DateOnly IssueDate { get; set; }
        public string? ExtractedBy { get; set; }
    }
}
