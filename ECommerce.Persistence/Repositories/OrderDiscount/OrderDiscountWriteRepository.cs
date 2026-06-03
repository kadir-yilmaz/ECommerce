using ECommerce.Application.Repositories.OrderDiscount;
using ECommerce.Domain.Entities;
using ECommerce.Persistence.Contexts;

namespace ECommerce.Persistence.Repositories
{
    public class OrderDiscountWriteRepository : WriteRepository<OrderDiscount>, IOrderDiscountWriteRepository
    {
        public OrderDiscountWriteRepository(ECommerceDbContext context) : base(context)
        {
        }
    }
}
