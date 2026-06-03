using ECommerce.Application.DTOs.Discount;
using ECommerce.Application.Features.Queries.Discount.CalculateCartDiscount;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ECommerce.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartDiscountController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CartDiscountController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("calculate")]
        public async Task<IActionResult> CalculateDiscount([FromBody] CalculateDiscountRequest request)
        {
            var query = new CalculateCartDiscountQueryRequest { DiscountRequest = request };
            var response = await _mediator.Send(query);
            return Ok(response.DiscountResponse);
        }
    }
}
