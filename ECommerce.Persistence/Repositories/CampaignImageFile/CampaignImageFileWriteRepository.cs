using ECommerce.Application.Repositories;
using ECommerce.Domain.Entities;
using ECommerce.Persistence.Contexts;

namespace ECommerce.Persistence.Repositories
{
    public class CampaignImageFileWriteRepository : WriteRepository<CampaignImageFile>, ICampaignImageFileWriteRepository
    {
        public CampaignImageFileWriteRepository(ECommerceDbContext context) : base(context)
        {
        }
    }
}
