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
                .WithMessage("Ad alanı boş bırakılamaz.")
                .MaximumLength(50)
                .WithMessage("Ad 50 karakterden uzun olamaz.");

            RuleFor(x => x.LastName)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("Soyad alanı boş bırakılamaz.")
                .MaximumLength(50)
                .WithMessage("Soyad 50 karakterden uzun olamaz.");

            RuleFor(x => x.Email)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("E-posta adresi zorunludur.")
                .EmailAddress()
                .WithMessage("Geçerli bir e-posta formatı giriniz.");

            RuleFor(x => x.PhoneNumber)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("Telefon numarası zorunludur.")
                .Matches(@"^[0-9\s]+$")
                .WithMessage("Telefon numarası sadece rakam ve boşluk içermelidir.")
                .Must(p =>
                 {
                    var digitsOnly = new string(p.Where(char.IsDigit).ToArray());
                    if (digitsOnly.StartsWith("0")) digitsOnly = digitsOnly.Substring(1);

                    return digitsOnly.Length == 10;
                  }).WithMessage("Telefon numarası geçersiz. Lütfen 10 hane (5xx...) olacak şekilde giriniz.");


            RuleFor(x => x.Password)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("Şifre boş olamaz.")
                .MinimumLength(8)
                .WithMessage("Şifre en az 8 karakter olmalıdır.")
                .Matches(@"[A-Z]+")
                .WithMessage("Şifre en az bir büyük harf içermelidir.")
                .Matches(@"[a-z]+")
                .WithMessage("Şifre en az bir küçük harf içermelidir.")
                .Matches(@"[0-9]+")
                .WithMessage("Şifre en az bir rakam içermelidir.")
                .Matches(@"[\!\?\*\.]+")
                .WithMessage("Şifre en az bir özel karakter (!?*.) içermelidir.");
        }
    }
}
