using Microsoft.Extensions.DependencyInjection;
using Wallet.Application.Mappings;

namespace Wallet.Application.DependencyResolvers
{
    public static class MapperServiceInjection
    {
        public static void AddMapperService(this IServiceCollection services)
        {
            services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>());
        }
    }
}
