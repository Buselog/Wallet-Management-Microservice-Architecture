using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Wallet.Application.Validators;

namespace Wallet.Application.DependencyResolvers
{
    public static class ValidatorServiceInjection
    {
        public static void AddValidatorServices(this IServiceCollection services)
        {
            services.AddValidatorsFromAssemblyContaining<CreateWalletRequestValidator>();
        }
    }
}
