using ECommerce.Application.Repositories.OrderDiscount;
using ECommerce.Domain.Entities;
using ECommerce.Persistence.Contexts;

namespace ECommerce.Persistence.Repositories
{
    public class OrderDiscountReadRepository : ReadRepository<OrderDiscount>, IOrderDiscountReadRepository
    {
        public OrderDiscountReadRepository(ECommerceDbContext context) : base(context)
        {
        }
    }
}
