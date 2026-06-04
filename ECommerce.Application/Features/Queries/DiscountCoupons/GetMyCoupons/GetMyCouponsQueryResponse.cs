using System;
using System.Collections.Generic;

namespace ECommerce.Application.Features.Queries.DiscountCoupons.GetMyCoupons
{
    public class GetMyCouponsQueryResponse
    {
        public List<MyCouponDto> Coupons { get; set; }
    }

    public class MyCouponDto
    {
        public string Id { get; set; }
        public string CouponId { get; set; }
        public string Code { get; set; }
        public string DiscountType { get; set; }
        public decimal DiscountValue { get; set; }
        public decimal? MaxDiscountAmount { get; set; }
        public decimal MinCartAmount { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public bool IsUsed { get; set; }
        public DateTime? UsedDate { get; set; }
        public bool IsRewardCoupon { get; set; }
        public bool IsExpired { get; set; }
    }
}
