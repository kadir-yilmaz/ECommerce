using ECommerce.Application.Repositories;
using ECommerce.Domain.Entities;
using ECommerce.Persistence.Contexts;

namespace ECommerce.Persistence.Repositories
{
    public class CampaignImageFileReadRepository : ReadRepository<CampaignImageFile>, ICampaignImageFileReadRepository
    {
        public CampaignImageFileReadRepository(ECommerceDbContext context) : base(context)
        {
        }
    }
}
