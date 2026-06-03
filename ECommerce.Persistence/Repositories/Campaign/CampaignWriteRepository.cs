using ECommerce.Application.Repositories.Campaign;
using ECommerce.Domain.Entities;
using ECommerce.Persistence.Contexts;

namespace ECommerce.Persistence.Repositories
{
    public class CampaignWriteRepository : WriteRepository<Campaign>, ICampaignWriteRepository
    {
        public CampaignWriteRepository(ECommerceDbContext context) : base(context)
        {
        }
    }
}
