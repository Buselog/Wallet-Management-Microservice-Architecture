using Investment.Application.Helpers;
using Investment.Application.Services;
using Investment.InnerInfrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Investment.InnerInfrastructure.DependencyResolvers
{
    public static class ManagerServiceInjection
    {
        public static void AddServices(this IServiceCollection services)
        {
            services.AddScoped<ICacheService, RedisCacheService>();
            services.AddScoped<ITradeService, TradeService>();
            services.AddSingleton<IReferenceGenerator, ReferenceGenerator>();
        }
    }
}
