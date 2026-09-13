
namespace Document.Application.Exceptions
{
    public class InvoiceAlreadyProcessedException : BaseBusinessException
    {
        public InvoiceAlreadyProcessedException() : base("ERR_INVOICE_ALREADY_PROCESSED")
        {

        }
    }
}
