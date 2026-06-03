using ECommerce.Application.Repositories.DiscountCoupon;
using ECommerce.Domain.Entities;
using ECommerce.Persistence.Contexts;

namespace ECommerce.Persistence.Repositories
{
    public class DiscountCouponReadRepository : ReadRepository<DiscountCoupon>, IDiscountCouponReadRepository
    {
        public DiscountCouponReadRepository(ECommerceDbContext context) : base(context)
        {
        }
    }
}
