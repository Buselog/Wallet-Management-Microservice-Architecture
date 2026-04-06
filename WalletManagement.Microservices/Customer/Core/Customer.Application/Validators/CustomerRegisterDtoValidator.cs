using Customer.Application.Dtos;
using FluentValidation;

namespace Customer.Application.Validators
{
    public class CustomerRegisterDtoValidator : AbstractValidator<CustomerRegisterDto>
    {
        public CustomerRegisterDtoValidator()
        {
            RuleFor(x => x.FirstName)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithErrorCode("ERR_NAME_FIELD_CANNOT_BE_EMPTY")
                .MaximumLength(50)
                .WithErrorCode("ERR_NAME_CANNOT_LONGER_THAN_50_CHARACHTERS");

            RuleFor(x => x.LastName)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithErrorCode("ERR_SURNAME_FIELD_CANNOT_BE_EMPTY")
                .MaximumLength(50)
                .WithErrorCode("ERR_SURNAME_CANNOT_LONGER_THAN_50_CHARACHTERS");

            RuleFor(x => x.Email)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithErrorCode("ERR_EMAIL_ADDRESS_CANNOT_EMPTY")
                .EmailAddress()
                .WithErrorCode("ERR_INVALID_EMAIL_ADDRESS");

            RuleFor(x => x.PhoneNumber)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithErrorCode("ERR_PHONE_NUMBER_CANNOT_BE_EMPTY")
                .Matches(@"^[0-9\s]+$")
                .WithErrorCode("ERR_PHONE_NUMBER_DOESNT_MATCH_STANDART_FORMAT")
                .Must(p =>
                 {
                    var digitsOnly = new string(p.Where(char.IsDigit).ToArray());
                    if (digitsOnly.StartsWith("0")) digitsOnly = digitsOnly.Substring(1);

                    return digitsOnly.Length == 10;
                  }).WithErrorCode("ERR_INVALID_PHONE_NUMBER");


            RuleFor(x => x.Password)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithErrorCode("ERR_PASSWORD_CANNOT_EMPTY")
                .MinimumLength(8)
                .WithErrorCode("ERR_PASSWORD_MUST_BE_AT_LEAST_8_CHARACHTERS_LONG")
                .Matches(@"[A-Z]+")
                .WithErrorCode("ERR_PASSWORD_MUST_CONTAIN_UPPERCASE_CHARACTER")
                .Matches(@"[a-z]+")
                .WithErrorCode("ERR_PASSWORD_MUST_CONTAIN_LOWERCASE_CHARACTER")
                .Matches(@"[0-9]+")
                .WithErrorCode("ERR_PASSWORD_MUST_CONTAIN_ONE_DIGIT")
                .Matches(@"[\!\?\*\.]+")
                .WithErrorCode("ERR_PASSWORD_MUST_CONTAIN_SPECIAL_CHARACHTER");
        }
    }
}
