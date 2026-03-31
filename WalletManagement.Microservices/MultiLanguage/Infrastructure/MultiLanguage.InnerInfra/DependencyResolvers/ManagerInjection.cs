using Microsoft.Extensions.DependencyInjection;
using MultiLanguage.Application.Managers;
using MultiLanguage.InnerInfra.Managers;

namespace MultiLanguage.InnerInfra.DependencyResolvers
{
    public static class ManagerInjection
    {
        public static void AddManagerServices(this IServiceCollection services)
        {
            services.AddScoped<ILanguageManager, LanguageManager>();
            services.AddScoped<IResourceManager, ResourceManager>();
        }
    }
}
