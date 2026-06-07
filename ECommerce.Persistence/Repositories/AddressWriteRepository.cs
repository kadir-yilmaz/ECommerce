using ECommerce.Application.Repositories;
using ECommerce.Domain.Entities;
using ECommerce.Persistence.Contexts;

namespace ECommerce.Persistence.Repositories
{
    public class AddressWriteRepository : WriteRepository<Address>, IAddressWriteRepository
    {
        public AddressWriteRepository(ECommerceDbContext context) : base(context)
        {
        }
    }
}
