using ECommerce.Application.Repositories.RewardRule;
using ECommerce.Domain.Entities;
using ECommerce.Persistence.Contexts;

namespace ECommerce.Persistence.Repositories
{
    public class RewardRuleWriteRepository : WriteRepository<RewardRule>, IRewardRuleWriteRepository
    {
        public RewardRuleWriteRepository(ECommerceDbContext context) : base(context)
        {
        }
    }
}
