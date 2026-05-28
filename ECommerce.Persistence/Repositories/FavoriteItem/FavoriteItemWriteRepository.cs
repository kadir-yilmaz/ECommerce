using ECommerce.Application.Repositories;
using ECommerce.Domain.Entities;
using ECommerce.Persistence.Contexts;

namespace ECommerce.Persistence.Repositories
{
    public class FavoriteItemWriteRepository : WriteRepository<FavoriteItem>, IFavoriteItemWriteRepository
    {
        public FavoriteItemWriteRepository(ECommerceDbContext context) : base(context)
        {
        }
    }
}
