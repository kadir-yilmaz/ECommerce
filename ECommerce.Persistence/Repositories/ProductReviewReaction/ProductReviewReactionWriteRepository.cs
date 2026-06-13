using ECommerce.Application.Repositories.ProductReviewReaction;
using ECommerce.Domain.Entities;
using ECommerce.Persistence.Contexts;
using ECommerce.Persistence.Repositories;

namespace ECommerce.Persistence.Repositories.ProductReviewReaction
{
    public class ProductReviewReactionWriteRepository : WriteRepository<Domain.Entities.ProductReviewReaction>, IProductReviewReactionWriteRepository
    {
        public ProductReviewReactionWriteRepository(ECommerceDbContext context) : base(context)
        {
        }
    }
}
