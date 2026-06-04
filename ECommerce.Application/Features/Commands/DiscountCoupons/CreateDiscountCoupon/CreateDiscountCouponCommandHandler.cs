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
        private readonly ECommerce.Application.Repositories.UserCoupon.IUserCouponWriteRepository _userCouponWriteRepository;

        public CreateDiscountCouponCommandHandler(
            IDiscountCouponWriteRepository discountCouponWriteRepository,
            ECommerce.Application.Repositories.UserCoupon.IUserCouponWriteRepository userCouponWriteRepository)
        {
            _discountCouponWriteRepository = discountCouponWriteRepository;
            _userCouponWriteRepository = userCouponWriteRepository;
        }

        public async Task<CreateDiscountCouponCommandResponse> Handle(CreateDiscountCouponCommandRequest request, CancellationToken cancellationToken)
        {
            var coupon = new DiscountCoupon
            {
                Code = request.Code,
                DiscountType = request.DiscountType,
                DiscountValue = request.DiscountValue,
                MaxDiscountAmount = request.MaxDiscountAmount,
                MinCartAmount = request.MinCartAmount,
                IsActive = request.IsActive,
                ExpirationDate = request.ExpirationDate,
                UsedCount = 0,
                Scope = request.Scope
            };

            await _discountCouponWriteRepository.AddAsync(coupon);
            await _discountCouponWriteRepository.SaveAsync();

            if (request.Scope == "Private" && request.UserIds != null && request.UserIds.Count > 0)
            {
                foreach (var userId in request.UserIds)
                {
                    await _userCouponWriteRepository.AddAsync(new ECommerce.Domain.Entities.UserCoupon
                    {
                        UserId = userId,
                        DiscountCouponId = coupon.Id,
                        IsUsed = false
                    });
                }
                await _userCouponWriteRepository.SaveAsync();
            }

            return new CreateDiscountCouponCommandResponse
            {
                Success = true,
                Message = "Kupon başarıyla eklendi."
            };
        }
    }
}
