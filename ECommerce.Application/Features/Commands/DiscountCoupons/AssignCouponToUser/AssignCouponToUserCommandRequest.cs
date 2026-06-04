using MediatR;
using System.Collections.Generic;

namespace ECommerce.Application.Features.Commands.DiscountCoupons.AssignCouponToUser
{
    public class AssignCouponToUserCommandRequest : IRequest<AssignCouponToUserCommandResponse>
    {
        public string CouponId { get; set; }
        public List<string> UserIds { get; set; }
    }
}
