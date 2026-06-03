using MediatR;

namespace ECommerce.Application.Features.Commands.DiscountCoupons.DeleteDiscountCoupon
{
    public class DeleteDiscountCouponCommandRequest : IRequest<DeleteDiscountCouponCommandResponse>
    {
        public string Id { get; set; }
    }
}
