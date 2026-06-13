using ECommerce.Application.Repositories.ProductReviewReaction;
using ECommerce.Domain.Entities;
using ECommerce.Persistence.Contexts;
using ECommerce.Persistence.Repositories;

namespace ECommerce.Persistence.Repositories.ProductReviewReaction
{
    public class ProductReviewReactionReadRepository : ReadRepository<Domain.Entities.ProductReviewReaction>, IProductReviewReactionReadRepository
    {
        public ProductReviewReactionReadRepository(ECommerceDbContext context) : base(context)
        {
        }
    }
}
