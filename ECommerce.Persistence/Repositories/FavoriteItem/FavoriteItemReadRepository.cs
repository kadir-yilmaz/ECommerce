using ECommerce.Application.Repositories;
using ECommerce.Domain.Entities;
using ECommerce.Persistence.Contexts;

namespace ECommerce.Persistence.Repositories
{
    public class FavoriteItemReadRepository : ReadRepository<FavoriteItem>, IFavoriteItemReadRepository
    {
        public FavoriteItemReadRepository(ECommerceDbContext context) : base(context)
        {
        }
    }
}
