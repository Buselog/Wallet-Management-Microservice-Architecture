using Customer.Application.Managers;
using Customer.InnerInfrastructure.Managers;
using Microsoft.Extensions.DependencyInjection;


namespace Customer.InnerInfrastructure.DependencyResolvers
{
    public static class ManagerServiceInjection
    {
        public static void AddManagerServices(this IServiceCollection services)
        {
            services.AddScoped<IAuthManager, AuthManager>();
            services.AddScoped<ICustomerManager, CustomerManager>();
        }
    }
}
