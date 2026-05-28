using ECommerce.Application.Repositories;
using ECommerce.Domain.Entities;
using ECommerce.Persistence.Contexts;

namespace ECommerce.Persistence.Repositories
{
    public class FavoriteReadRepository : ReadRepository<Favorite>, IFavoriteReadRepository
    {
        public FavoriteReadRepository(ECommerceDbContext context) : base(context)
        {
        }
    }
}
