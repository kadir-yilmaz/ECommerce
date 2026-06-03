using ECommerce.Application.Repositories.DiscountCoupon;
using ECommerce.Domain.Entities;
using ECommerce.Persistence.Contexts;

namespace ECommerce.Persistence.Repositories
{
    public class DiscountCouponWriteRepository : WriteRepository<DiscountCoupon>, IDiscountCouponWriteRepository
    {
        public DiscountCouponWriteRepository(ECommerceDbContext context) : base(context)
        {
        }
    }
}
