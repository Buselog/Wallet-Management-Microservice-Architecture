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
                .WithMessage("Cüzdan seçimi zorunludur.")
                .GreaterThan(0)
                .WithMessage("Lütfen geçerli bir cüzdan seçiniz.");

            RuleFor(x => x.Amount)
                .Cascade(CascadeMode.Stop)
                .NotNull()
                .WithMessage("Miktar alanı boş bırakılamaz.")
                .GreaterThan(0)
                .WithMessage("Yatırılacak tutar 0'dan büyük olmalıdır.");

            RuleFor(x => x.ReferenceId)
                .NotEmpty()
                .WithMessage("Referans numarası zorunludur.");
        }
    }
}
