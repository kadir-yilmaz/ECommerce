using ECommerce.Application.Repositories.DiscountCoupon;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace ECommerce.Application.Features.Commands.DiscountCoupons.DeleteDiscountCoupon
{
    public class DeleteDiscountCouponCommandHandler : IRequestHandler<DeleteDiscountCouponCommandRequest, DeleteDiscountCouponCommandResponse>
    {
        private readonly IDiscountCouponWriteRepository _discountCouponWriteRepository;

        public DeleteDiscountCouponCommandHandler(IDiscountCouponWriteRepository discountCouponWriteRepository)
        {
            _discountCouponWriteRepository = discountCouponWriteRepository;
        }

        public async Task<DeleteDiscountCouponCommandResponse> Handle(DeleteDiscountCouponCommandRequest request, CancellationToken cancellationToken)
        {
            await _discountCouponWriteRepository.RemoveAsync(request.Id);
            await _discountCouponWriteRepository.SaveAsync();

            return new DeleteDiscountCouponCommandResponse { Success = true, Message = "Kupon silindi." };
        }
    }
}
