using MediatR;
using ECommerce.Application.DTOs.Discount;

namespace ECommerce.Application.Features.Queries.Discount.CalculateCartDiscount
{
    public class CalculateCartDiscountQueryRequest : IRequest<CalculateCartDiscountQueryResponse>
    {
        public CalculateDiscountRequest DiscountRequest { get; set; }
    }
}
