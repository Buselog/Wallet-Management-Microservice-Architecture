using FluentValidation;
using Wallet.Application.Dtos;

namespace Wallet.Application.Validators
{
    public class CreateWalletRequestValidator : AbstractValidator<CreateWalletRequestDto>
    {
        public CreateWalletRequestValidator()
        {
            RuleFor(x => x.CustomerNo)
                .NotEmpty()
                .WithMessage("Müşteri numarası boş olamaz.");

            RuleFor(x => x.Currency)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("Para birimi alanı boş bırakılamaz.")
                .Length(3)
                .WithMessage("Para birimi 3 karakter olmalıdır (örn: TRY).");

            RuleFor(x => x.Type)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("Cüzdan tipi seçimi boş bırakılamaz.")
                .IsInEnum()
                .WithMessage("Geçersiz cüzdan tipi.");
        }
    }
}
