using MediatR;
using System;

namespace ECommerce.Application.Features.Commands.DiscountCoupons.UpdateDiscountCoupon
{
    public class UpdateDiscountCouponCommandRequest : IRequest<UpdateDiscountCouponCommandResponse>
    {
        public string Id { get; set; }
        public string Code { get; set; }
        public string DiscountType { get; set; }
        public decimal DiscountValue { get; set; }
        public decimal MinCartAmount { get; set; }
        public bool IsActive { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public int? UsageLimit { get; set; }
    }
}
