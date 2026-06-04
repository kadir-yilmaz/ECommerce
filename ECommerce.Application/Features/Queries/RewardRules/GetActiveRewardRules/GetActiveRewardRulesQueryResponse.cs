using ECommerce.Application.Features.Queries.RewardRules.GetAllRewardRules;
using System.Collections.Generic;

namespace ECommerce.Application.Features.Queries.RewardRules.GetActiveRewardRules
{
    public class GetActiveRewardRulesQueryResponse
    {
        public List<RewardRuleDto> Rules { get; set; }
    }
}
