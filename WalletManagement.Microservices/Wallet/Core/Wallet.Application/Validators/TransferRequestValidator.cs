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
                .WithMessage("Cüzdan seçimi zorunludur.")
                .GreaterThan(0)
                .WithMessage("Geçerli bir gönderici cüzdan seçilmelidir.");

            RuleFor(x => x.Target)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("Alıcı (IBAN veya Telefon No) boş bırakılamaz.")
                .Matches(@"^(TR[A-Z0-9]{24}|[0-9\s\(\)\-]+)$")
                .WithMessage("Alıcı bilgisi ya geçerli bir IBAN ya da geçerli bir telefon numarası olmalıdır.")
                .Must(y =>
                {
                    if (y.StartsWith("TR", StringComparison.OrdinalIgnoreCase))
                        return y.Length == 26;

                    var digitsOnly = new string(y.Where(char.IsDigit).ToArray());

                    if (digitsOnly.StartsWith("0")) digitsOnly = digitsOnly.Substring(1);

                    return digitsOnly.Length == 10; 
                })
               .WithMessage("Alıcı bilgisi ya 'TR' ile başlayan bir IBAN ya da geçerli bir telefon numarası olmalıdır.");

            RuleFor(x => x.Amount)
                .Cascade(CascadeMode.Stop)
                .NotNull()
                .WithMessage("Miktar alanı boş bırakılamaz.")
                .GreaterThan(0)
                .WithMessage("Transfer tutarı 0'dan büyük olmalıdır.");

            RuleFor(x => x.ReferenceId)
                .NotEmpty()
                .WithMessage("Referans numarası alanı zorunludur.");
        }
    }
}
