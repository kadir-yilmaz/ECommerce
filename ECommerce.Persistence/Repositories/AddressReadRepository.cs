using ECommerce.Application.Repositories;
using ECommerce.Domain.Entities;
using ECommerce.Persistence.Contexts;

namespace ECommerce.Persistence.Repositories
{
    public class AddressReadRepository : ReadRepository<Address>, IAddressReadRepository
    {
        public AddressReadRepository(ECommerceDbContext context) : base(context)
        {
        }
    }
}
