using FluentValidation;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Wallet.WebAPI.Filters
{
    public class ValidationFilter : IActionFilter
    {
        private readonly IServiceProvider _serviceProvider;

        public ValidationFilter(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var dto = context.ActionArguments.Values
                .FirstOrDefault(v => v != null && v.GetType().Name.EndsWith("Dto"));

            if (dto == null) return;

            var validatorType = typeof(IValidator<>).MakeGenericType(dto.GetType());
            var validator = _serviceProvider.GetService(validatorType) as IValidator;

            if (validator != null)
            {
                var result = validator.Validate(new ValidationContext<object>(dto));

                if (!result.IsValid)
                {
                    throw new FluentValidation.ValidationException(result.Errors);
                }
            }
        }

        public void OnActionExecuted(ActionExecutedContext context) { }
    }
}
