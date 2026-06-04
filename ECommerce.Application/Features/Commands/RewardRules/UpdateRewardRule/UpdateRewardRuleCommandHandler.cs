using ECommerce.Application.Repositories.RewardRule;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace ECommerce.Application.Features.Commands.RewardRules.UpdateRewardRule
{
    public class UpdateRewardRuleCommandHandler : IRequestHandler<UpdateRewardRuleCommandRequest, UpdateRewardRuleCommandResponse>
    {
        private readonly IRewardRuleReadRepository _rewardRuleReadRepository;
        private readonly IRewardRuleWriteRepository _rewardRuleWriteRepository;

        public UpdateRewardRuleCommandHandler(IRewardRuleReadRepository rewardRuleReadRepository, IRewardRuleWriteRepository rewardRuleWriteRepository)
        {
            _rewardRuleReadRepository = rewardRuleReadRepository;
            _rewardRuleWriteRepository = rewardRuleWriteRepository;
        }

        public async Task<UpdateRewardRuleCommandResponse> Handle(UpdateRewardRuleCommandRequest request, CancellationToken cancellationToken)
        {
            var rule = await _rewardRuleReadRepository.GetByIdAsync(request.Id);
            if (rule == null)
                return new UpdateRewardRuleCommandResponse { Success = false, Message = "Ödül kuralı bulunamadı." };

            rule.Name = request.Name;
            rule.PeriodInDays = request.PeriodInDays;
            rule.MinTotalSpent = request.MinTotalSpent;
            rule.RewardDiscountType = request.RewardDiscountType;
            rule.RewardDiscountValue = request.RewardDiscountValue;
            rule.RewardMaxDiscountAmount = request.RewardMaxDiscountAmount;
            rule.CouponValidityDays = request.CouponValidityDays;
            rule.CouponMinCartAmount = request.CouponMinCartAmount;
            rule.IsActive = request.IsActive;

            _rewardRuleWriteRepository.Update(rule);
            await _rewardRuleWriteRepository.SaveAsync();

            return new UpdateRewardRuleCommandResponse { Success = true, Message = "Ödül kuralı güncellendi." };
        }
    }
}
