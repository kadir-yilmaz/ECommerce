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
    public class CampaignsController : ControllerBase
    {
        readonly IMediator _mediator;

        public CampaignsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = "Admin")]
        [AuthorizeDefinition(Menu = AuthorizeDefinitionConstants.Campaigns, ActionType = ActionType.Writing, Definition = "Create Campaign")]
        public async Task<IActionResult> Create([FromBody] ECommerce.Application.Features.Commands.Campaigns.CreateCampaign.CreateCampaignCommandRequest createCampaignCommandRequest)
        {
            var response = await _mediator.Send(createCampaignCommandRequest);
            return Ok(response);
        }

        [HttpPut]
        [Authorize(AuthenticationSchemes = "Admin")]
        [AuthorizeDefinition(Menu = AuthorizeDefinitionConstants.Campaigns, ActionType = ActionType.Updating, Definition = "Update Campaign")]
        public async Task<IActionResult> Update([FromBody] ECommerce.Application.Features.Commands.Campaigns.UpdateCampaign.UpdateCampaignCommandRequest updateCampaignCommandRequest)
        {
            var response = await _mediator.Send(updateCampaignCommandRequest);
            return Ok(response);
        }

        [HttpDelete("{Id}")]
        [Authorize(AuthenticationSchemes = "Admin")]
        [AuthorizeDefinition(Menu = AuthorizeDefinitionConstants.Campaigns, ActionType = ActionType.Deleting, Definition = "Delete Campaign")]
        public async Task<IActionResult> Delete([FromRoute] ECommerce.Application.Features.Commands.Campaigns.DeleteCampaign.DeleteCampaignCommandRequest deleteCampaignCommandRequest)
        {
            var response = await _mediator.Send(deleteCampaignCommandRequest);
            return Ok(response);
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = "Admin")]
        [AuthorizeDefinition(Menu = AuthorizeDefinitionConstants.Campaigns, ActionType = ActionType.Reading, Definition = "Get All Campaigns")]
        public async Task<IActionResult> GetAll([FromQuery] ECommerce.Application.Features.Queries.Campaigns.GetAllCampaigns.GetAllCampaignsQueryRequest request)
        {
            var response = await _mediator.Send(request);
            return Ok(response);
        }

        [HttpGet("active")]
        [AllowAnonymous]
        public async Task<IActionResult> GetActiveCampaigns([FromQuery] ECommerce.Application.Features.Queries.Campaigns.GetActiveCampaigns.GetActiveCampaignsQueryRequest request)
        {
            var response = await _mediator.Send(request);
            return Ok(response);
        }

        [HttpGet("detail/{Id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCampaignById([FromRoute] ECommerce.Application.Features.Queries.Campaigns.GetCampaignById.GetCampaignByIdQueryRequest request)
        {
            var response = await _mediator.Send(request);
            return Ok(response);
        }
    }
}
