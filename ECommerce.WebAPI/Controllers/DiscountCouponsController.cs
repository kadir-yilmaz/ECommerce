using ECommerce.Application.Consts;
using ECommerce.Application.CustomAttributes;
using ECommerce.Application.Enums;
using ECommerce.Application.Features.Commands.DiscountCoupons.AssignCouponToUser;
using ECommerce.Application.Features.Commands.DiscountCoupons.CreateDiscountCoupon;
using ECommerce.Application.Features.Commands.DiscountCoupons.DeleteDiscountCoupon;
using ECommerce.Application.Features.Commands.DiscountCoupons.UpdateDiscountCoupon;
using ECommerce.Application.Features.Queries.DiscountCoupons.GetAllDiscountCoupons;
using ECommerce.Application.Features.Queries.DiscountCoupons.GetMyCoupons;
using ECommerce.Application.Features.Queries.DiscountCoupons.GetPublicCoupons;
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
        public async Task<IActionResult> Create([FromBody] CreateDiscountCouponCommandRequest request)
        {
            var response = await _mediator.Send(request);
            return Ok(response);
        }

        [HttpPut]
        [AuthorizeDefinition(Menu = AuthorizeDefinitionConstants.Campaigns, ActionType = ActionType.Updating, Definition = "Update Discount Coupon")]
        public async Task<IActionResult> Update([FromBody] UpdateDiscountCouponCommandRequest request)
        {
            var response = await _mediator.Send(request);
            return Ok(response);
        }

        [HttpDelete("{Id}")]
        [AuthorizeDefinition(Menu = AuthorizeDefinitionConstants.Campaigns, ActionType = ActionType.Deleting, Definition = "Delete Discount Coupon")]
        public async Task<IActionResult> Delete([FromRoute] DeleteDiscountCouponCommandRequest request)
        {
            var response = await _mediator.Send(request);
            return Ok(response);
        }

        [HttpGet]
        [AuthorizeDefinition(Menu = AuthorizeDefinitionConstants.Campaigns, ActionType = ActionType.Reading, Definition = "Get All Discount Coupons")]
        public async Task<IActionResult> GetAll([FromQuery] GetAllDiscountCouponsQueryRequest request)
        {
            var response = await _mediator.Send(request);
            return Ok(response);
        }

        [HttpPost("assign-to-users")]
        [AuthorizeDefinition(Menu = AuthorizeDefinitionConstants.Campaigns, ActionType = ActionType.Writing, Definition = "Assign Coupon To Users")]
        public async Task<IActionResult> AssignToUsers([FromBody] AssignCouponToUserCommandRequest request)
        {
            var response = await _mediator.Send(request);
            return Ok(response);
        }

        [HttpGet("public")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPublicCoupons()
        {
            var response = await _mediator.Send(new GetPublicCouponsQueryRequest());
            return Ok(response);
        }

        [HttpGet("my-coupons")]
        public async Task<IActionResult> GetMyCoupons()
        {
            var response = await _mediator.Send(new GetMyCouponsQueryRequest());
            return Ok(response);
        }
    }
}
