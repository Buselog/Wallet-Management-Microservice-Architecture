

namespace Document.Application.Dtos
{
    public class InvoiceExtractionResultDto
    {
        public bool IsReceiptOrInvoice { get; set; }

        public string BillerName { get; set; }

        public string InvoiceNumber { get; set; }

        public decimal TotalAmount { get; set; }

        public DateTime IssueDate { get; set; }

        public string ExtractedBy { get; set; }
    }
}
