using Customer.Contract.Repositories;
using Customer.Persistence.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Customer.Persistence.DependencyResolvers
{
    public static class PersistenceServiceInjection
    {
        public static void AddPersistenceServices(this IServiceCollection services)
        {
            services.AddScoped<ICustomerRepository, CustomerRepository>();
        }
    }
}
