using Microsoft.Extensions.DependencyInjection;
using Wallet.Application.Managers;
using Wallet.InnerInfrastructure.Managers;

namespace Wallet.InnerInfrastructure.DependencyResolvers
{
    public static class ManagerServiceInjection
    {
        public static void AddManagerServices(this IServiceCollection services)
        {
            services.AddScoped<IWalletManager, WalletManager>();
            services.AddScoped<ITransactionManager, TransactionManager>();
        }
    }
}
