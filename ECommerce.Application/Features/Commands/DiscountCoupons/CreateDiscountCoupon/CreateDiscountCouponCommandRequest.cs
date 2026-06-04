using MediatR;
using System;
using System.Collections.Generic;

namespace ECommerce.Application.Features.Commands.DiscountCoupons.CreateDiscountCoupon
{
    public class CreateDiscountCouponCommandRequest : IRequest<CreateDiscountCouponCommandResponse>
    {
        public string Code { get; set; }
        public string DiscountType { get; set; }
        public decimal DiscountValue { get; set; }
        public decimal? MaxDiscountAmount { get; set; }
        public decimal MinCartAmount { get; set; }
        public bool IsActive { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public string Scope { get; set; } = "Public";
        public List<string>? UserIds { get; set; }
    }
}
