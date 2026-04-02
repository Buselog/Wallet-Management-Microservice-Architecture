using Microsoft.Extensions.DependencyInjection;
using MultiLanguage.Application.Mappings;

namespace MultiLanguage.Application.DependencyResolvers
{
    public static class MapperInjection
    {
        public static void AddMapperService(this IServiceCollection services)
        {
            services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>());
        }
    }
}
