using Microsoft.Extensions.DependencyInjection;
using MultiLanguage.Application.Services;
using MultiLanguage.InnerInfra.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiLanguage.InnerInfra.DependencyResolvers
{
    public static class ServiceInjection
    {
        public static void AddExternalServices(this IServiceCollection services)
        {
            services.AddScoped<ICacheService, RedisCacheService>();
        }
    }
}

