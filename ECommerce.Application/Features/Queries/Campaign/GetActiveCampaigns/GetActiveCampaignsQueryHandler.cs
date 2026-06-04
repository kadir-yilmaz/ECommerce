using ECommerce.Application.Repositories.Campaign;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ECommerce.Application.Features.Queries.Campaigns.GetActiveCampaigns
{
    public class GetActiveCampaignsQueryHandler : IRequestHandler<GetActiveCampaignsQueryRequest, GetActiveCampaignsQueryResponse>
    {
        private readonly ICampaignReadRepository _campaignReadRepository;

        public GetActiveCampaignsQueryHandler(ICampaignReadRepository campaignReadRepository)
        {
            _campaignReadRepository = campaignReadRepository;
        }

        public async Task<GetActiveCampaignsQueryResponse> Handle(GetActiveCampaignsQueryRequest request, CancellationToken cancellationToken)
        {
            var activeCampaigns = await _campaignReadRepository.GetWhere(c => c.IsActive && (c.EndDate == null || c.EndDate > DateTime.UtcNow))
                .Select(c => new GetActiveCampaignsQueryResponse.CampaignDto
                {
                    Id = c.Id.ToString(),
                    Name = c.Name,
                    Description = c.Description,
                    RuleType = c.RuleType,
                    MinAmount = c.MinAmount,
                    DiscountRate = c.DiscountRate,
                    MinQuantity = c.MinQuantity,
                    FreeQuantity = c.FreeQuantity,
                    ProductId = c.ProductId,
                    CategoryId = c.CategoryId,
                    Brand = c.Brand,
                    EndDate = c.EndDate
                })
                .ToListAsync(cancellationToken);

            return new GetActiveCampaignsQueryResponse
            {
                Campaigns = activeCampaigns
            };
        }
    }
}
