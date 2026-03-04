using Customer.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace Customer.Persistence.DependencyResolvers
{
    public static class DbContextServiceInjection 
    {
        public static void AddDbContextService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<CustomerContext>(
                opt => opt.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection")));
        }
    }
}
