using ECommerce.Application.Repositories.Category;
using ECommerce.Persistence.Contexts;

namespace ECommerce.Persistence.Repositories.Category
{
    public class CategoryReadRepository : ReadRepository<ECommerce.Domain.Entities.Category>, ICategoryReadRepository
    {
        public CategoryReadRepository(ECommerceDbContext context) : base(context)
        {
        }
    }
}
