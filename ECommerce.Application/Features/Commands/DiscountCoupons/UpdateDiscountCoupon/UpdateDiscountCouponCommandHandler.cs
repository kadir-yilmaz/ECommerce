using ECommerce.Application.Repositories.DiscountCoupon;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace ECommerce.Application.Features.Commands.DiscountCoupons.UpdateDiscountCoupon
{
    public class UpdateDiscountCouponCommandHandler : IRequestHandler<UpdateDiscountCouponCommandRequest, UpdateDiscountCouponCommandResponse>
    {
        private readonly IDiscountCouponWriteRepository _discountCouponWriteRepository;
        private readonly IDiscountCouponReadRepository _discountCouponReadRepository;

        public UpdateDiscountCouponCommandHandler(IDiscountCouponWriteRepository discountCouponWriteRepository, IDiscountCouponReadRepository discountCouponReadRepository)
        {
            _discountCouponWriteRepository = discountCouponWriteRepository;
            _discountCouponReadRepository = discountCouponReadRepository;
        }

        public async Task<UpdateDiscountCouponCommandResponse> Handle(UpdateDiscountCouponCommandRequest request, CancellationToken cancellationToken)
        {
            var coupon = await _discountCouponReadRepository.GetByIdAsync(request.Id);
            if (coupon == null)
            {
                return new UpdateDiscountCouponCommandResponse { Success = false, Message = "Kupon bulunamadı." };
            }

            coupon.Code = request.Code;
            coupon.DiscountType = request.DiscountType;
            coupon.DiscountValue = request.DiscountValue;
            coupon.MinCartAmount = request.MinCartAmount;
            coupon.IsActive = request.IsActive;
            coupon.ExpirationDate = request.ExpirationDate;
            coupon.UsageLimit = request.UsageLimit;

            _discountCouponWriteRepository.Update(coupon);
            await _discountCouponWriteRepository.SaveAsync();

            return new UpdateDiscountCouponCommandResponse { Success = true, Message = "Kupon güncellendi." };
        }
    }
}
