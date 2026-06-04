using ECommerce.Application.Repositories.RewardRule;
using ECommerce.Domain.Entities;
using ECommerce.Persistence.Contexts;

namespace ECommerce.Persistence.Repositories
{
    public class RewardRuleReadRepository : ReadRepository<RewardRule>, IRewardRuleReadRepository
    {
        public RewardRuleReadRepository(ECommerceDbContext context) : base(context)
        {
        }
    }
}
