using Customer.Application.Validators;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Customer.Application.DependencyResolvers
{
    public static class ValidatorServiceInjection
    {
        public static void AddValidatorServices(this IServiceCollection services)
        {
            services.AddValidatorsFromAssemblyContaining<CustomerRegisterDtoValidator>();
        }
    }
}
