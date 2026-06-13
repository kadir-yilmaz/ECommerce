using ECommerce.Application.Repositories.ProductReview;
using ECommerce.Domain.Entities;
using ECommerce.Persistence.Contexts;

namespace ECommerce.Persistence.Repositories.ProductReview
{
    public class ProductReviewReadRepository : ReadRepository<Domain.Entities.ProductReview>, IProductReviewReadRepository
    {
        public ProductReviewReadRepository(ECommerceDbContext context) : base(context)
        {
        }
    }
}
