using ECommerce.Application.Repositories.UserCoupon;
using ECommerce.Domain.Entities;
using ECommerce.Persistence.Contexts;

namespace ECommerce.Persistence.Repositories
{
    public class UserCouponReadRepository : ReadRepository<UserCoupon>, IUserCouponReadRepository
    {
        public UserCouponReadRepository(ECommerceDbContext context) : base(context)
        {
        }
    }
}
