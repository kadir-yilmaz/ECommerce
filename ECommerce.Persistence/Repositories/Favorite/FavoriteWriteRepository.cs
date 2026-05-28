using ECommerce.Application.Repositories;
using ECommerce.Domain.Entities;
using ECommerce.Persistence.Contexts;

namespace ECommerce.Persistence.Repositories
{
    public class FavoriteWriteRepository : WriteRepository<Favorite>, IFavoriteWriteRepository
    {
        public FavoriteWriteRepository(ECommerceDbContext context) : base(context)
        {
        }
    }
}
