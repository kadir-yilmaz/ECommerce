using ECommerce.Application.Repositories.UserCoupon;
using ECommerce.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ECommerce.Application.Features.Queries.DiscountCoupons.GetMyCoupons
{
    public class GetMyCouponsQueryHandler : IRequestHandler<GetMyCouponsQueryRequest, GetMyCouponsQueryResponse>
    {
        private readonly IUserCouponReadRepository _userCouponReadRepository;
        private readonly UserManager<ECommerce.Domain.Entities.AppUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GetMyCouponsQueryHandler(
            IUserCouponReadRepository userCouponReadRepository,
            UserManager<ECommerce.Domain.Entities.AppUser> userManager,
            IHttpContextAccessor httpContextAccessor)
        {
            _userCouponReadRepository = userCouponReadRepository;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<GetMyCouponsQueryResponse> Handle(GetMyCouponsQueryRequest request, CancellationToken cancellationToken)
        {
            var username = _httpContextAccessor.HttpContext?.User?.Identity?.Name;
            if (string.IsNullOrEmpty(username))
                return new GetMyCouponsQueryResponse { Coupons = new() };

            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
                return new GetMyCouponsQueryResponse { Coupons = new() };

            var now = DateTime.UtcNow;

            var coupons = await _userCouponReadRepository.GetWhere(uc => uc.UserId == user.Id, false)
                .Include(uc => uc.DiscountCoupon)
                .Where(uc => uc.DiscountCoupon.IsActive)
                .Select(uc => new MyCouponDto
                {
                    Id = uc.Id.ToString(),
                    CouponId = uc.DiscountCouponId.ToString(),
                    Code = uc.DiscountCoupon.Code,
                    DiscountType = uc.DiscountCoupon.DiscountType,
                    DiscountValue = uc.DiscountCoupon.DiscountValue,
                    MaxDiscountAmount = uc.DiscountCoupon.MaxDiscountAmount,
                    MinCartAmount = uc.DiscountCoupon.MinCartAmount,
                    ExpirationDate = uc.DiscountCoupon.ExpirationDate,
                    IsUsed = uc.IsUsed,
                    UsedDate = uc.UsedDate,
                    IsRewardCoupon = uc.DiscountCoupon.IsRewardCoupon,
                    IsExpired = uc.DiscountCoupon.ExpirationDate != null && uc.DiscountCoupon.ExpirationDate < now
                })
                .OrderByDescending(c => c.IsUsed == false)
                .ThenByDescending(c => c.ExpirationDate)
                .ToListAsync(cancellationToken);

            return new GetMyCouponsQueryResponse { Coupons = coupons };
        }
    }
}
