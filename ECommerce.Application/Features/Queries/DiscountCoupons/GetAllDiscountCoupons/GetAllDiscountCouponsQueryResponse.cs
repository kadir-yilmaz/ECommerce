using System;
using System.Collections.Generic;

namespace ECommerce.Application.Features.Queries.DiscountCoupons.GetAllDiscountCoupons
{
    public class GetAllDiscountCouponsQueryResponse
    {
        public List<DiscountCouponDto> Coupons { get; set; }
    }

    public class DiscountCouponDto
    {
        public string Id { get; set; }
        public string Code { get; set; }
        public string DiscountType { get; set; }
        public decimal DiscountValue { get; set; }
        public decimal? MaxDiscountAmount { get; set; }
        public decimal MinCartAmount { get; set; }
        public bool IsActive { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public int UsedCount { get; set; }
        public string Scope { get; set; }
        public bool IsRewardCoupon { get; set; }
        public List<CouponUserDto> AssignedUsers { get; set; } = new List<CouponUserDto>();
    }

    public class CouponUserDto
    {
        public string UserName { get; set; }
        public bool IsUsed { get; set; }
    }
}

