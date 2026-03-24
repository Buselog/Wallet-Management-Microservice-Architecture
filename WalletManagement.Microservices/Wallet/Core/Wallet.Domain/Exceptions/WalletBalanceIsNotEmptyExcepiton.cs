namespace Wallet.Domain.Exceptions
{
    public class WalletBalanceIsNotEmptyExcepiton : BaseBusinessException
    {

        public WalletBalanceIsNotEmptyExcepiton() 
            : base("Silmek istediğiniz cüzdanda para bulunmaktadır. Silmek için lütfen parayı başka bir cüzdanınıza aktarınız.")
        {

        }
    }
}
