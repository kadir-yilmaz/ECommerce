using System;
using System.Collections.Generic;

namespace ECommerce.Application.Features.Queries.DiscountCoupons.GetPublicCoupons
{
    public class GetPublicCouponsQueryResponse
    {
        public List<PublicCouponDto> Coupons { get; set; }
    }

    public class PublicCouponDto
    {
        public string Id { get; set; }
        public string Code { get; set; }
        public string DiscountType { get; set; }
        public decimal DiscountValue { get; set; }
        public decimal? MaxDiscountAmount { get; set; }
        public decimal MinCartAmount { get; set; }
        public DateTime? ExpirationDate { get; set; }
    }
}
