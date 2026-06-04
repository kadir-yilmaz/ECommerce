using ECommerce.Application.Repositories.DiscountCoupon;
using ECommerce.Application.Repositories.UserCoupon;
using ECommerce.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ECommerce.Application.Features.Commands.DiscountCoupons.AssignCouponToUser
{
    public class AssignCouponToUserCommandHandler : IRequestHandler<AssignCouponToUserCommandRequest, AssignCouponToUserCommandResponse>
    {
        private readonly IDiscountCouponReadRepository _discountCouponReadRepository;
        private readonly IUserCouponWriteRepository _userCouponWriteRepository;
        private readonly IUserCouponReadRepository _userCouponReadRepository;

        public AssignCouponToUserCommandHandler(
            IDiscountCouponReadRepository discountCouponReadRepository,
            IUserCouponWriteRepository userCouponWriteRepository,
            IUserCouponReadRepository userCouponReadRepository)
        {
            _discountCouponReadRepository = discountCouponReadRepository;
            _userCouponWriteRepository = userCouponWriteRepository;
            _userCouponReadRepository = userCouponReadRepository;
        }

        public async Task<AssignCouponToUserCommandResponse> Handle(AssignCouponToUserCommandRequest request, CancellationToken cancellationToken)
        {
            var coupon = await _discountCouponReadRepository.GetByIdAsync(request.CouponId);
            if (coupon == null)
                return new AssignCouponToUserCommandResponse { Success = false, Message = "Kupon bulunamadı." };

            int assignedCount = 0;
            foreach (var userId in request.UserIds)
            {
                // Aynı kullanıcıya aynı kupon daha önce atanmış mı kontrol et
                var existing = await _userCouponReadRepository.GetWhere(
                    uc => uc.UserId == userId && uc.DiscountCouponId == coupon.Id, false
                ).FirstOrDefaultAsync(cancellationToken);

                if (existing == null)
                {
                    await _userCouponWriteRepository.AddAsync(new UserCoupon
                    {
                        DiscountCouponId = coupon.Id,
                        UserId = userId,
                        IsUsed = false
                    });
                    assignedCount++;
                }
            }

            await _userCouponWriteRepository.SaveAsync();

            return new AssignCouponToUserCommandResponse
            {
                Success = true,
                Message = $"{assignedCount} kullanıcıya kupon atandı."
            };
        }
    }
}
