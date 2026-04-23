using FluentValidation;
using Wallet.Application.Dtos;

namespace Wallet.Application.Validators
{
    public class CreateWalletRequestValidator : AbstractValidator<CreateWalletRequestDto>
    {
        public CreateWalletRequestValidator()
        {
            RuleFor(x => x.Currency)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithErrorCode("ERR_CURRENCY_CANNOT_BLANK")
                .Length(3)
                .WithErrorCode("ERR_CURRENCY_NAME_CHARACTERS");

            RuleFor(x => x.Type)
                .Cascade(CascadeMode.Stop)
                .NotNull()
                .WithErrorCode("ERR_WALLET_TYPE_CANNOT_BLANK")
                .IsInEnum()
                .WithErrorCode("ERR_INVALID_WALLET_TYPE");
        }
    }
}
