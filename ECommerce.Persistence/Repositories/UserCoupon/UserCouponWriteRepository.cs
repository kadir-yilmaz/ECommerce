using ECommerce.Application.Repositories.UserCoupon;
using ECommerce.Domain.Entities;
using ECommerce.Persistence.Contexts;

namespace ECommerce.Persistence.Repositories
{
    public class UserCouponWriteRepository : WriteRepository<UserCoupon>, IUserCouponWriteRepository
    {
        public UserCouponWriteRepository(ECommerceDbContext context) : base(context)
        {
        }
    }
}
