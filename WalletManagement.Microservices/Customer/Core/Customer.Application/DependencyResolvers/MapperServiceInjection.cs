using Customer.Application.Mappings;
using Microsoft.Extensions.DependencyInjection;

namespace Customer.Application.DependencyResolvers
{
    public static class MapperServiceInjection
    {
        public static void AddMapperService(this IServiceCollection services)
        {
            services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>());
        }
    }
}
