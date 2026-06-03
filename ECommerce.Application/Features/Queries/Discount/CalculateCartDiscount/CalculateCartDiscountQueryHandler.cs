using ECommerce.Application.Abstractions.Discount;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace ECommerce.Application.Features.Queries.Discount.CalculateCartDiscount
{
    public class CalculateCartDiscountQueryHandler : IRequestHandler<CalculateCartDiscountQueryRequest, CalculateCartDiscountQueryResponse>
    {
        private readonly IDiscountService _discountService;

        public CalculateCartDiscountQueryHandler(IDiscountService discountService)
        {
            _discountService = discountService;
        }

        public async Task<CalculateCartDiscountQueryResponse> Handle(CalculateCartDiscountQueryRequest request, CancellationToken cancellationToken)
        {
            var response = await _discountService.CalculateDiscountAsync(request.DiscountRequest);
            return new CalculateCartDiscountQueryResponse
            {
                DiscountResponse = response
            };
        }
    }
}
