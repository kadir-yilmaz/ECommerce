using ECommerce.Application.Consts;
using ECommerce.Application.CustomAttributes;
using ECommerce.Application.Enums;
using ECommerce.Application.Features.Commands.CampaignImageFile.RemoveCampaignImage;
using ECommerce.Application.Features.Commands.CampaignImageFile.UploadCampaignImage;
using ECommerce.Application.Features.Queries.CampaignImageFile.GetCampaignImages;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Get([FromQuery] GetCampaignImagesQueryRequest getCampaignImagesQueryRequest)
        {
            var response = await _mediator.Send(getCampaignImagesQueryRequest);
            return Ok(response);
        }

        [HttpPost("[action]")]
        [Authorize(AuthenticationSchemes = "Admin")]
        [AuthorizeDefinition(Menu = AuthorizeDefinitionConstants.Campaigns, ActionType = ActionType.Writing, Definition = "Upload Campaign Image")]
        public async Task<IActionResult> Upload([FromQuery] UploadCampaignImageCommandRequest uploadCampaignImageCommandRequest)
        {
            uploadCampaignImageCommandRequest.Files = Request.Form.Files;
            var response = await _mediator.Send(uploadCampaignImageCommandRequest);
            return Ok(response);
        }

        [HttpPut("update-title")]
        [Authorize(AuthenticationSchemes = "Admin")]
        [AuthorizeDefinition(Menu = AuthorizeDefinitionConstants.Campaigns, ActionType = ActionType.Updating, Definition = "Update Campaign Image Title")]
        public async Task<IActionResult> UpdateTitle([FromBody] ECommerce.Application.Features.Commands.CampaignImageFile.UpdateCampaignImageTitle.UpdateCampaignImageTitleCommandRequest updateCampaignImageTitleCommandRequest)
        {
            var response = await _mediator.Send(updateCampaignImageTitleCommandRequest);
            return Ok(response);
        }

        [HttpDelete("{Id}")]
        [Authorize(AuthenticationSchemes = "Admin")]
        [AuthorizeDefinition(Menu = AuthorizeDefinitionConstants.Campaigns, ActionType = ActionType.Deleting, Definition = "Remove Campaign Image")]
        public async Task<IActionResult> Delete([FromRoute] RemoveCampaignImageCommandRequest removeCampaignImageCommandRequest)
        {
            var response = await _mediator.Send(removeCampaignImageCommandRequest);
            return Ok(response);
        }
    }
}
