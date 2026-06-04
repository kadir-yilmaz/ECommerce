using MediatR;

namespace ECommerce.Application.Features.Commands.RewardRules.CreateRewardRule
{
    public class CreateRewardRuleCommandRequest : IRequest<CreateRewardRuleCommandResponse>
    {
        public string Name { get; set; }
        public int PeriodInDays { get; set; }
        public decimal MinTotalSpent { get; set; }
        public string RewardDiscountType { get; set; }
        public decimal RewardDiscountValue { get; set; }
        public decimal? RewardMaxDiscountAmount { get; set; }
        public int CouponValidityDays { get; set; }
        public decimal CouponMinCartAmount { get; set; }
        public bool IsActive { get; set; }
    }
}
