using Customer.Application.Dtos;
using FluentValidation;

namespace Customer.Application.Validators
{
    public class CustomerLoginDtoValidator : AbstractValidator<CustomerLoginDto>
    {
        public CustomerLoginDtoValidator()
        {
            RuleFor(x => x.Email)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithErrorCode("ERR_EMAIL_ADDRESS_CANNOT_EMPTY")
                .EmailAddress()
                .WithErrorCode("ERR_INVALID_EMAIL_ADDRESS");

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithErrorCode("ERR_PASSWORD_CANNOT_EMPTY");
        }
    }
}
