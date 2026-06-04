using ECommerce.Application.Repositories.DiscountCoupon;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ECommerce.Application.Features.Queries.DiscountCoupons.GetPublicCoupons
{
    public class GetPublicCouponsQueryHandler : IRequestHandler<GetPublicCouponsQueryRequest, GetPublicCouponsQueryResponse>
    {
        private readonly IDiscountCouponReadRepository _discountCouponReadRepository;

        public GetPublicCouponsQueryHandler(IDiscountCouponReadRepository discountCouponReadRepository)
        {
            _discountCouponReadRepository = discountCouponReadRepository;
        }

        public async Task<GetPublicCouponsQueryResponse> Handle(GetPublicCouponsQueryRequest request, CancellationToken cancellationToken)
        {
            var coupons = await _discountCouponReadRepository.GetWhere(
                c => c.Scope == "Public"
                     && c.IsActive
                     && (c.ExpirationDate == null || c.ExpirationDate > DateTime.UtcNow)
                     && (c.UsageLimit == null || c.UsedCount < c.UsageLimit),
                false
            ).Select(c => new PublicCouponDto
            {
                Id = c.Id.ToString(),
                Code = c.Code,
                DiscountType = c.DiscountType,
                DiscountValue = c.DiscountValue,
                MaxDiscountAmount = c.MaxDiscountAmount,
                MinCartAmount = c.MinCartAmount,
                ExpirationDate = c.ExpirationDate
            }).ToListAsync(cancellationToken);

            return new GetPublicCouponsQueryResponse { Coupons = coupons };
        }
    }
}
