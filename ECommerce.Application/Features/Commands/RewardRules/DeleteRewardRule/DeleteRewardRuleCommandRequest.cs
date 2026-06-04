using MediatR;

namespace ECommerce.Application.Features.Commands.RewardRules.DeleteRewardRule
{
    public class DeleteRewardRuleCommandRequest : IRequest<DeleteRewardRuleCommandResponse>
    {
        public string Id { get; set; }
    }
}
