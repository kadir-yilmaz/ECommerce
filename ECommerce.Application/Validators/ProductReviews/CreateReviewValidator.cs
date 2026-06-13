using ECommerce.Application.Features.Commands.ProductReview.CreateReview;
using FluentValidation;

namespace ECommerce.Application.Validators.ProductReviews
{
    public class CreateReviewValidator : AbstractValidator<CreateReviewCommandRequest>
    {
        public CreateReviewValidator()
        {
            RuleFor(r => r.ProductId)
                .NotEmpty()
                    .WithMessage("Ürün ID boş olamaz.");

            RuleFor(r => r.Rating)
                .InclusiveBetween(1, 5)
                    .WithMessage("Puan 1 ile 5 arasında olmalıdır.");

            RuleFor(r => r.Comment)
                .NotEmpty()
                .NotNull()
                    .WithMessage("Yorum metni boş olamaz.")
                .MinimumLength(10)
                    .WithMessage("Yorum en az 10 karakter olmalıdır.")
                .MaximumLength(1000)
                    .WithMessage("Yorum en fazla 1000 karakter olabilir.");
        }
    }
}
