using ECommerce.Application.Consts;
using ECommerce.Application.CustomAttributes;
using ECommerce.Application.Enums;
using ECommerce.Application.Features.Commands.RewardRules.CreateRewardRule;
using ECommerce.Application.Features.Commands.RewardRules.DeleteRewardRule;
using ECommerce.Application.Features.Commands.RewardRules.UpdateRewardRule;
using ECommerce.Application.Features.Queries.RewardRules.GetAllRewardRules;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ECommerce.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Admin")]
    public class RewardRulesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public RewardRulesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [AuthorizeDefinition(Menu = AuthorizeDefinitionConstants.RewardRules, ActionType = ActionType.Writing, Definition = "Create Reward Rule")]
        public async Task<IActionResult> Create([FromBody] CreateRewardRuleCommandRequest request)
        {
            var response = await _mediator.Send(request);
            return Ok(response);
        }

        [HttpPut]
        [AuthorizeDefinition(Menu = AuthorizeDefinitionConstants.RewardRules, ActionType = ActionType.Updating, Definition = "Update Reward Rule")]
        public async Task<IActionResult> Update([FromBody] UpdateRewardRuleCommandRequest request)
        {
            var response = await _mediator.Send(request);
            return Ok(response);
        }

        [HttpDelete("{Id}")]
        [AuthorizeDefinition(Menu = AuthorizeDefinitionConstants.RewardRules, ActionType = ActionType.Deleting, Definition = "Delete Reward Rule")]
        public async Task<IActionResult> Delete([FromRoute] DeleteRewardRuleCommandRequest request)
        {
            var response = await _mediator.Send(request);
            return Ok(response);
        }

        [HttpGet]
        [AuthorizeDefinition(Menu = AuthorizeDefinitionConstants.RewardRules, ActionType = ActionType.Reading, Definition = "Get All Reward Rules")]
        public async Task<IActionResult> GetAll()
        {
            var response = await _mediator.Send(new GetAllRewardRulesQueryRequest());
            return Ok(response);
        }

        [HttpGet("active")]
        [AllowAnonymous]
        public async Task<IActionResult> GetActive()
        {
            var response = await _mediator.Send(new ECommerce.Application.Features.Queries.RewardRules.GetActiveRewardRules.GetActiveRewardRulesQueryRequest());
            return Ok(response);
        }
    }
}
