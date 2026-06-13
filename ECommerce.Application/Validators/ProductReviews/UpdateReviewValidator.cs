using ECommerce.Application.Features.Commands.ProductReview.UpdateReview;
using FluentValidation;

namespace ECommerce.Application.Validators.ProductReviews
{
    public class UpdateReviewValidator : AbstractValidator<UpdateReviewCommandRequest>
    {
        public UpdateReviewValidator()
        {
            RuleFor(r => r.ReviewId)
                .NotEmpty()
                    .WithMessage("Yorum ID boş olamaz.");

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
