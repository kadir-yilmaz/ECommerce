using ECommerce.Application.Features.Queries.RewardRules.GetAllRewardRules;
using ECommerce.Application.Repositories.RewardRule;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ECommerce.Application.Features.Queries.RewardRules.GetActiveRewardRules
{
    public class GetActiveRewardRulesQueryHandler : IRequestHandler<GetActiveRewardRulesQueryRequest, GetActiveRewardRulesQueryResponse>
    {
        private readonly IRewardRuleReadRepository _rewardRuleReadRepository;

        public GetActiveRewardRulesQueryHandler(IRewardRuleReadRepository rewardRuleReadRepository)
        {
            _rewardRuleReadRepository = rewardRuleReadRepository;
        }

        public async Task<GetActiveRewardRulesQueryResponse> Handle(GetActiveRewardRulesQueryRequest request, CancellationToken cancellationToken)
        {
            var rules = await _rewardRuleReadRepository.GetWhere(r => r.IsActive, false)
                .OrderByDescending(r => r.CreatedDate)
                .Select(r => new RewardRuleDto
                {
                    Id = r.Id.ToString(),
                    Name = r.Name,
                    PeriodInDays = r.PeriodInDays,
                    MinTotalSpent = r.MinTotalSpent,
                    RewardDiscountType = r.RewardDiscountType,
                    RewardDiscountValue = r.RewardDiscountValue,
                    RewardMaxDiscountAmount = r.RewardMaxDiscountAmount,
                    CouponValidityDays = r.CouponValidityDays,
                    CouponMinCartAmount = r.CouponMinCartAmount,
                    IsActive = r.IsActive,
                    CreatedDate = r.CreatedDate
                }).ToListAsync(cancellationToken);

            return new GetActiveRewardRulesQueryResponse { Rules = rules };
        }
    }
}
