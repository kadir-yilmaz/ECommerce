using ECommerce.Application.Repositories.RewardRule;
using ECommerce.Domain.Entities;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace ECommerce.Application.Features.Commands.RewardRules.CreateRewardRule
{
    public class CreateRewardRuleCommandHandler : IRequestHandler<CreateRewardRuleCommandRequest, CreateRewardRuleCommandResponse>
    {
        private readonly IRewardRuleWriteRepository _rewardRuleWriteRepository;

        public CreateRewardRuleCommandHandler(IRewardRuleWriteRepository rewardRuleWriteRepository)
        {
            _rewardRuleWriteRepository = rewardRuleWriteRepository;
        }

        public async Task<CreateRewardRuleCommandResponse> Handle(CreateRewardRuleCommandRequest request, CancellationToken cancellationToken)
        {
            await _rewardRuleWriteRepository.AddAsync(new RewardRule
            {
                Name = request.Name,
                PeriodInDays = request.PeriodInDays,
                MinTotalSpent = request.MinTotalSpent,
                RewardDiscountType = request.RewardDiscountType,
                RewardDiscountValue = request.RewardDiscountValue,
                RewardMaxDiscountAmount = request.RewardMaxDiscountAmount,
                CouponValidityDays = request.CouponValidityDays,
                CouponMinCartAmount = request.CouponMinCartAmount,
                IsActive = request.IsActive
            });

            await _rewardRuleWriteRepository.SaveAsync();

            return new CreateRewardRuleCommandResponse
            {
                Success = true,
                Message = "Ödül kuralı başarıyla oluşturuldu."
            };
        }
    }
}
