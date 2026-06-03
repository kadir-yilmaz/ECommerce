using ECommerce.Application.Repositories.DiscountCoupon;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ECommerce.Application.Features.Queries.DiscountCoupons.GetAllDiscountCoupons
{
    public class GetAllDiscountCouponsQueryHandler : IRequestHandler<GetAllDiscountCouponsQueryRequest, GetAllDiscountCouponsQueryResponse>
    {
        private readonly IDiscountCouponReadRepository _discountCouponReadRepository;

        public GetAllDiscountCouponsQueryHandler(IDiscountCouponReadRepository discountCouponReadRepository)
        {
            _discountCouponReadRepository = discountCouponReadRepository;
        }

        public async Task<GetAllDiscountCouponsQueryResponse> Handle(GetAllDiscountCouponsQueryRequest request, CancellationToken cancellationToken)
        {
            var coupons = await _discountCouponReadRepository.GetAll(false).Select(c => new DiscountCouponDto
            {
                Id = c.Id.ToString(),
                Code = c.Code,
                DiscountType = c.DiscountType,
                DiscountValue = c.DiscountValue,
                MinCartAmount = c.MinCartAmount,
                IsActive = c.IsActive,
                ExpirationDate = c.ExpirationDate,
                UsageLimit = c.UsageLimit,
                UsedCount = c.UsedCount
            }).ToListAsync(cancellationToken);

            return new GetAllDiscountCouponsQueryResponse
            {
                Coupons = coupons
            };
        }
    }
}
