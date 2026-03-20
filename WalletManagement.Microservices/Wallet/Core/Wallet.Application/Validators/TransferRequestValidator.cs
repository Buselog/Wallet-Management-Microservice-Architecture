using FluentValidation;
using Wallet.Application.Dtos;

namespace Wallet.Application.Validators
{
    public class TransferRequestValidator : AbstractValidator<TransferRequestDto>
    {
        public TransferRequestValidator()
        {
            RuleFor(x => x.FromWalletId)
                .GreaterThan(0)
                .WithMessage("Geçerli bir gönderici cüzdan seçilmelidir.");

            RuleFor(x => x.Target)
                .NotEmpty()
                .WithMessage("Alıcı (IBAN veya Müşteri No) boş bırakılamaz.");

            RuleFor(x => x.Amount)
                .GreaterThan(0)
                .WithMessage("Transfer tutarı 0'dan büyük olmalıdır.");

            RuleFor(x => x.ReferenceId)
                .NotEmpty()
                .WithMessage("Referans numarası alanı zorunludur.");
        }
    }
}
