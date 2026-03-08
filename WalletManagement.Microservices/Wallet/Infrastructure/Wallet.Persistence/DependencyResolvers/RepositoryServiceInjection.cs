using Microsoft.Extensions.DependencyInjection;
using Wallet.Contract.Repositories;
using Wallet.Persistence.Repositories;

namespace Wallet.Persistence.DependencyResolvers
{
    public static class RepositoryServiceInjection
    {
        public static void AddRepositoryServices(this IServiceCollection services)
        {
            services.AddScoped<IWalletRepository, WalletRepository>();
            services.AddScoped<ITransactionRepository, WalletTransactionRepository>();

        }
    }
}
