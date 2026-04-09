using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Customer.WebAPI.Filters
{
    public class ValidationFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var failures = context.ModelState
                   .Where(x => x.Value.Errors.Count > 0)
                   .SelectMany(x => x.Value.Errors.Select(error => new ValidationFailure(x.Key, error.ErrorMessage)))
                   .ToList();

                throw new FluentValidation.ValidationException(failures);
            }
        }

        public void OnActionExecuted(ActionExecutedContext context) { 
        
        }
    }
}
