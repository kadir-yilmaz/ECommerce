using ECommerce.Application.Repositories.Campaign;
using ECommerce.Domain.Entities;
using ECommerce.Persistence.Contexts;

namespace ECommerce.Persistence.Repositories
{
    public class CampaignReadRepository : ReadRepository<Campaign>, ICampaignReadRepository
    {
        public CampaignReadRepository(ECommerceDbContext context) : base(context)
        {
        }
    }
}
