
namespace Customer.Application.Managers
{
    public interface IWalletServiceClient
    {
        Task CreateDefaultWalletAsync(string customerNo);
    }
}
