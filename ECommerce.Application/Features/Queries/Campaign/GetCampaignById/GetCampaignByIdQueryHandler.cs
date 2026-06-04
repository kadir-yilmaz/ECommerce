using ECommerce.Application.Repositories.Campaign;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace ECommerce.Application.Features.Queries.Campaigns.GetCampaignById
{
    public class GetCampaignByIdQueryHandler : IRequestHandler<GetCampaignByIdQueryRequest, GetCampaignByIdQueryResponse>
    {
        private readonly ICampaignReadRepository _campaignReadRepository;

        public GetCampaignByIdQueryHandler(ICampaignReadRepository campaignReadRepository)
        {
            _campaignReadRepository = campaignReadRepository;
        }

        public async Task<GetCampaignByIdQueryResponse> Handle(GetCampaignByIdQueryRequest request, CancellationToken cancellationToken)
        {
            var campaign = await _campaignReadRepository.GetByIdAsync(request.Id, false);

            if (campaign == null)
            {
                return null;
            }

            return new GetCampaignByIdQueryResponse
            {
                Id = campaign.Id.ToString(),
                Name = campaign.Name,
                Description = campaign.Description,
                RuleType = campaign.RuleType,
                MinAmount = campaign.MinAmount,
                DiscountRate = campaign.DiscountRate,
                MinQuantity = campaign.MinQuantity,
                FreeQuantity = campaign.FreeQuantity,
                ProductId = campaign.ProductId,
                CategoryId = campaign.CategoryId,
                Brand = campaign.Brand,
                EndDate = campaign.EndDate
            };
        }
    }
}
