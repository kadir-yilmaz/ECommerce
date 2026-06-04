using ECommerce.Application.Repositories.RewardRule;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace ECommerce.Application.Features.Commands.RewardRules.DeleteRewardRule
{
    public class DeleteRewardRuleCommandHandler : IRequestHandler<DeleteRewardRuleCommandRequest, DeleteRewardRuleCommandResponse>
    {
        private readonly IRewardRuleWriteRepository _rewardRuleWriteRepository;

        public DeleteRewardRuleCommandHandler(IRewardRuleWriteRepository rewardRuleWriteRepository)
        {
            _rewardRuleWriteRepository = rewardRuleWriteRepository;
        }

        public async Task<DeleteRewardRuleCommandResponse> Handle(DeleteRewardRuleCommandRequest request, CancellationToken cancellationToken)
        {
            var result = await _rewardRuleWriteRepository.RemoveAsync(request.Id);
            if (!result)
                return new DeleteRewardRuleCommandResponse { Success = false, Message = "Ödül kuralı bulunamadı." };

            await _rewardRuleWriteRepository.SaveAsync();

            return new DeleteRewardRuleCommandResponse { Success = true, Message = "Ödül kuralı silindi." };
        }
    }
}
