using ECommerce.Application.Repositories.Campaign;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ECommerce.Application.Features.Queries.Campaigns.GetAllCampaigns
{
    public class GetAllCampaignsQueryHandler : IRequestHandler<GetAllCampaignsQueryRequest, GetAllCampaignsQueryResponse>
    {
        private readonly ICampaignReadRepository _campaignReadRepository;

        public GetAllCampaignsQueryHandler(ICampaignReadRepository campaignReadRepository)
        {
            _campaignReadRepository = campaignReadRepository;
        }

        public async Task<GetAllCampaignsQueryResponse> Handle(GetAllCampaignsQueryRequest request, CancellationToken cancellationToken)
        {
            var campaigns = await _campaignReadRepository.GetAll(false).Select(c => new CampaignDto
            {
                Id = c.Id.ToString(),
                Name = c.Name,
                Description = c.Description,
                RuleType = c.RuleType.ToString(),
                MinAmount = c.MinAmount,
                DiscountRate = c.DiscountRate,
                MinQuantity = c.MinQuantity,
                FreeQuantity = c.FreeQuantity,
                ProductId = c.ProductId != null ? c.ProductId.ToString() : null,
                CategoryId = c.CategoryId != null ? c.CategoryId.ToString() : null,
                Brand = c.Brand,
                EndDate = c.EndDate,
                IsActive = c.IsActive
            }).ToListAsync(cancellationToken);

            return new GetAllCampaignsQueryResponse
            {
                Campaigns = campaigns
            };
        }
    }
}
