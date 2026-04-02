using FluentValidation;
using Wallet.Application.Dtos;

namespace Wallet.Application.Validators
{
    public class TransferRequestValidator : AbstractValidator<TransferRequestDto>
    {
        public TransferRequestValidator()
        {
            RuleFor(x => x.FromWalletId)
                .Cascade(CascadeMode.Stop)
                .NotNull()
                .WithErrorCode("ERR_WALLET_IS_NOT_CHOSEN")
                .GreaterThan(0)
                .WithErrorCode("ERR_INVALID_WALLET");

            RuleFor(x => x.Target)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithErrorCode("ERR_TARGET_CANNOT_EMPTY")
                .Matches(@"^(TR[A-Z0-9]{24}|[0-9\s\(\)\-]+)$")
                .WithErrorCode("ERR_TARGET_IS_NOT_VALID")
                .Must(y =>
                {
                    if (y.StartsWith("TR", StringComparison.OrdinalIgnoreCase))
                        return y.Length == 26;

                    var digitsOnly = new string(y.Where(char.IsDigit).ToArray());

                    if (digitsOnly.StartsWith("0")) digitsOnly = digitsOnly.Substring(1);

                    return digitsOnly.Length == 10; 
                })
               .WithErrorCode("ERR_TARGET_IS_NOT_VALID_FOR_SECOND_VALIDATION");

            RuleFor(x => x.Amount)
                .Cascade(CascadeMode.Stop)
                .NotNull()
                .WithErrorCode("ERR_AMOUNT_CANNOT_BE_BLANK")
                .GreaterThan(0)
                .WithErrorCode("ERR_AMOUNT_FOR_TRANSFER_MUST_GREATHER_THAN_0");

            RuleFor(x => x.ReferenceId)
                .NotEmpty()
                .WithErrorCode("ERR_REFERENCE_NUMBER_CANNOT_EMPTY");
        }
    }
}
