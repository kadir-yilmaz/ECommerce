using ECommerce.Application.Consts;
using ECommerce.Application.CustomAttributes;
using ECommerce.Application.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ECommerce.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Admin")]
    public class DiscountCouponsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public DiscountCouponsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [AuthorizeDefinition(Menu = AuthorizeDefinitionConstants.Campaigns, ActionType = ActionType.Writing, Definition = "Create Discount Coupon")]
        public async Task<IActionResult> Create([FromBody] ECommerce.Application.Features.Commands.DiscountCoupons.CreateDiscountCoupon.CreateDiscountCouponCommandRequest request)
        {
            var response = await _mediator.Send(request);
            return Ok(response);
        }

        [HttpPut]
        [AuthorizeDefinition(Menu = AuthorizeDefinitionConstants.Campaigns, ActionType = ActionType.Updating, Definition = "Update Discount Coupon")]
        public async Task<IActionResult> Update([FromBody] ECommerce.Application.Features.Commands.DiscountCoupons.UpdateDiscountCoupon.UpdateDiscountCouponCommandRequest request)
        {
            var response = await _mediator.Send(request);
            return Ok(response);
        }

        [HttpDelete("{Id}")]
        [AuthorizeDefinition(Menu = AuthorizeDefinitionConstants.Campaigns, ActionType = ActionType.Deleting, Definition = "Delete Discount Coupon")]
        public async Task<IActionResult> Delete([FromRoute] ECommerce.Application.Features.Commands.DiscountCoupons.DeleteDiscountCoupon.DeleteDiscountCouponCommandRequest request)
        {
            var response = await _mediator.Send(request);
            return Ok(response);
        }

        [HttpGet]
        [AuthorizeDefinition(Menu = AuthorizeDefinitionConstants.Campaigns, ActionType = ActionType.Reading, Definition = "Get All Discount Coupons")]
        public async Task<IActionResult> GetAll([FromQuery] ECommerce.Application.Features.Queries.DiscountCoupons.GetAllDiscountCoupons.GetAllDiscountCouponsQueryRequest request)
        {
            var response = await _mediator.Send(request);
            return Ok(response);
        }
    }
}
