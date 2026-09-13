
namespace Document.Application.Exceptions
{
    public class InvoiceAmountNotFoundException : BaseBusinessException
    {
        public InvoiceAmountNotFoundException() : base("ERR_INVOICE_AMOUNT_NOT_FOUND")
        {

        }
    }
}
