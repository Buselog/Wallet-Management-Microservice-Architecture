using FluentValidation;
using Wallet.Application.Dtos;

namespace Wallet.Application.Validators
{
    public class DepositRequestValidator : AbstractValidator<DepositRequestDto>
    {
        public DepositRequestValidator()
        {
            RuleFor(x => x.WalletId)
                .Cascade(CascadeMode.Stop)
                .NotNull()
                .WithErrorCode("ERR_WALLET_IS_NOT_CHOSEN")
                .GreaterThan(0)
                .WithErrorCode("ERR_INVALID_WALLET");

            RuleFor(x => x.Amount)
                .Cascade(CascadeMode.Stop)
                .NotNull()
                .WithErrorCode("ERR_AMOUNT_CANNOT_BE_BLANK")
                .GreaterThan(0)
                .WithErrorCode("ERR_AMOUNT_FOR_DEPOSIT_MUST_GREATHER_THAN_0");

            RuleFor(x => x.ReferenceId)
                .NotEmpty()
                .WithErrorCode("ERR_REFERENCE_NUMBER_CANNOT_EMPTY");
        }
    }
}
