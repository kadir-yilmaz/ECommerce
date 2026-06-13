using ECommerce.Application.Repositories.ProductReview;
using ECommerce.Domain.Entities;
using ECommerce.Persistence.Contexts;

namespace ECommerce.Persistence.Repositories.ProductReview
{
    public class ProductReviewWriteRepository : WriteRepository<Domain.Entities.ProductReview>, IProductReviewWriteRepository
    {
        public ProductReviewWriteRepository(ECommerceDbContext context) : base(context)
        {
        }
    }
}
