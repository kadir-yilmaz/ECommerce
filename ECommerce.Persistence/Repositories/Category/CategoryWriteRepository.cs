using ECommerce.Application.Repositories.Category;
using ECommerce.Persistence.Contexts;

namespace ECommerce.Persistence.Repositories.Category
{
    public class CategoryWriteRepository : WriteRepository<ECommerce.Domain.Entities.Category>, ICategoryWriteRepository
    {
        public CategoryWriteRepository(ECommerceDbContext context) : base(context)
        {
        }
    }
}
