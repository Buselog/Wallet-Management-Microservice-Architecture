using Microsoft.Extensions.DependencyInjection;
using Wallet.Application.Factories;
using Wallet.Application.Managers;
using Wallet.InnerInfrastructure.Factories;
using Wallet.InnerInfrastructure.Managers;

namespace Wallet.InnerInfrastructure.DependencyResolvers
{
    public static class ManagerServiceInjection
    {
        public static void AddManagerServices(this IServiceCollection services)
        {
            services.AddScoped<IWalletManager, WalletManager>();
            services.AddScoped<ITransactionManager, TransactionManager>();
            services.AddScoped<IWalletFactory, WalletFactory>();
        }
    }
}
