
using Microsoft.Extensions.DependencyInjection;
using MultiLanguage.Contract.Repositories;
using MultiLanguage.Persistence.Repositories;

namespace MultiLanguage.Persistence.DependencyResolvers
{
    public static class RepositoryInjection 
    {
        public static void AddRepositoryServices(this IServiceCollection services)
        {
            services.AddScoped<ILanguageRepository, LanguageRepository>();
            services.AddScoped<IResourceRepository, ResourceRepository>();
        }
    }
}
