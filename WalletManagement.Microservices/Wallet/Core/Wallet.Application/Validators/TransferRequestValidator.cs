using FluentValidation;
using Wallet.Application.Dtos;

namespace Wallet.Application.Validators
{
    public class TransferRequestValidator : AbstractValidator<TransferRequestDto>
    {
        public TransferRequestValidator()
        {
            RuleFor(x => x.FromWalletId)
                .NotEmpty()
                .WithMessage("Gönderen cüzdan seçilmelidir.");

            RuleFor(x => x.Target)
                .NotEmpty()
                .WithMessage("Alıcı (IBAN veya Müşteri No) boş bırakılamaz.");

            RuleFor(x => x.Amount)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("Tutar alanı boş olamaz.")
                .GreaterThan(0)
                .WithMessage("Transfer tutarı 0'dan büyük olmalıdır.");

            RuleFor(x => x.ReferenceId)
                .NotEmpty()
                .WithMessage("Referans numarası alanı zorunludur.");
        }
    }
}
