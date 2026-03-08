using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Wallet.Persistence.Context;

namespace Wallet.Persistence.DependencyResolvers
{
    public static class DbContextServiceInjection
    {
        public static void AddDbContextService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<WalletContext>(
                opt => opt.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection")));
        }
    }
}
