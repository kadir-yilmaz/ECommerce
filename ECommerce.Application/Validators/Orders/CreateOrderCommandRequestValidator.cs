using ECommerce.Application.Features.Commands.Order.CreateOrder;
using FluentValidation;

namespace ECommerce.Application.Validators.Orders
{
    public class CreateOrderCommandRequestValidator : AbstractValidator<CreateOrderCommandRequest>
    {
        public CreateOrderCommandRequestValidator()
        {
            RuleFor(x => x.ContactName)
                .NotEmpty().WithMessage("Teslimat alici adi zorunludur.")
                .MinimumLength(3).WithMessage("Teslimat alici adi en az 3 karakter olmalidir.")
                .MaximumLength(100).WithMessage("Teslimat alici adi en fazla 100 karakter olabilir.");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Telefon numarasi zorunludur.")
                .Matches(@"^\d{10,11}$").WithMessage("Telefon numarasi 10 veya 11 haneli olmaldir.");

            RuleFor(x => x.City)
                .NotEmpty().WithMessage("Il bilgisi zorunludur.")
                .MaximumLength(60).WithMessage("Il bilgisi en fazla 60 karakter olabilir.");

            RuleFor(x => x.District)
                .NotEmpty().WithMessage("Ilce bilgisi zorunludur.")
                .MaximumLength(60).WithMessage("Ilce bilgisi en fazla 60 karakter olabilir.");

            RuleFor(x => x.Neighborhood)
                .NotEmpty().WithMessage("Mahalle bilgisi zorunludur.")
                .MaximumLength(100).WithMessage("Mahalle bilgisi en fazla 100 karakter olabilir.");

            RuleFor(x => x.PostalCode)
                .NotEmpty().WithMessage("Posta kodu zorunludur.")
                .Matches(@"^\d{5}$").WithMessage("Posta kodu 5 haneli olmaldir.");

            RuleFor(x => x.AddressLine)
                .NotEmpty().WithMessage("Acik adres zorunludur.")
                .MinimumLength(10).WithMessage("Acik adres en az 10 karakter olmalidir.")
                .MaximumLength(500).WithMessage("Acik adres en fazla 500 karakter olabilir.");

            RuleFor(x => x.CardHolderName)
                .NotEmpty().WithMessage("Kart uzerindeki ad soyad zorunludur.")
                .MinimumLength(3).WithMessage("Kart uzerindeki ad soyad en az 3 karakter olmalidir.")
                .MaximumLength(100).WithMessage("Kart uzerindeki ad soyad en fazla 100 karakter olabilir.");

            RuleFor(x => x.CardNumber)
                .NotEmpty().WithMessage("Kart numarasi zorunludur.")
                .Matches(@"^\d{16}$").WithMessage("Kart numarasi 16 haneli olmaldir.")
                .Must(BeValidCardNumber).WithMessage("Kart numarasi gecerli degil.");

            RuleFor(x => x.ExpireMonth)
                .NotEmpty().WithMessage("Son kullanma ayi zorunludur.")
                .Must(BeValidMonth).WithMessage("Son kullanma ayi gecerli degil.");

            RuleFor(x => x.ExpireYear)
                .NotEmpty().WithMessage("Son kullanma yili zorunludur.")
                .Must(BeValidYear).WithMessage("Son kullanma yili gecerli degil.");

            RuleFor(x => x)
                .Must(HaveFutureExpiryDate).WithMessage("Kartinizin son kullanma tarihi gecmis gorunuyor.");

            RuleFor(x => x.Cvv)
                .NotEmpty().WithMessage("CVV zorunludur.")
                .Matches(@"^\d{3,4}$").WithMessage("CVV 3 veya 4 haneli olmaldir.");

            RuleFor(x => x.Description)
                .MaximumLength(300).WithMessage("Siparis notu en fazla 300 karakter olabilir.");
        }

        private static bool BeValidMonth(string? month) =>
            int.TryParse(month, out var parsedMonth) && parsedMonth is >= 1 and <= 12;

        private static bool BeValidYear(string? year) =>
            int.TryParse(year, out var parsedYear) && parsedYear >= DateTime.UtcNow.Year;

        private static bool HaveFutureExpiryDate(CreateOrderCommandRequest request)
        {
            if (!int.TryParse(request.ExpireMonth, out var month) || !int.TryParse(request.ExpireYear, out var year))
                return false;

            var now = DateTime.UtcNow;
            if (year < now.Year)
                return false;

            if (year == now.Year && month < now.Month)
                return false;

            return month is >= 1 and <= 12;
        }

        private static bool BeValidCardNumber(string? cardNumber)
        {
            if (string.IsNullOrWhiteSpace(cardNumber) || !cardNumber.All(char.IsDigit))
                return false;

            var sum = 0;
            var alternate = false;

            for (var i = cardNumber.Length - 1; i >= 0; i--)
            {
                var n = cardNumber[i] - '0';
                if (alternate)
                {
                    n *= 2;
                    if (n > 9)
                        n -= 9;
                }

                sum += n;
                alternate = !alternate;
            }

            return sum % 10 == 0;
        }
    }
}
