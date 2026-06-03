using ECommerce.Application.Repositories.DiscountCoupon;
using ECommerce.Domain.Entities;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace ECommerce.Application.Features.Commands.DiscountCoupons.CreateDiscountCoupon
{
    public class CreateDiscountCouponCommandHandler : IRequestHandler<CreateDiscountCouponCommandRequest, CreateDiscountCouponCommandResponse>
    {
        private readonly IDiscountCouponWriteRepository _discountCouponWriteRepository;

        public CreateDiscountCouponCommandHandler(IDiscountCouponWriteRepository discountCouponWriteRepository)
        {
            _discountCouponWriteRepository = discountCouponWriteRepository;
        }

        public async Task<CreateDiscountCouponCommandResponse> Handle(CreateDiscountCouponCommandRequest request, CancellationToken cancellationToken)
        {
            var coupon = new DiscountCoupon
            {
                Code = request.Code,
                DiscountType = request.DiscountType,
                DiscountValue = request.DiscountValue,
                MinCartAmount = request.MinCartAmount,
                IsActive = request.IsActive,
                ExpirationDate = request.ExpirationDate,
                UsageLimit = request.UsageLimit,
                UsedCount = 0
            };

            await _discountCouponWriteRepository.AddAsync(coupon);
            await _discountCouponWriteRepository.SaveAsync();

            return new CreateDiscountCouponCommandResponse
            {
                Success = true,
                Message = "Kupon başarıyla eklendi."
            };
        }
    }
}
