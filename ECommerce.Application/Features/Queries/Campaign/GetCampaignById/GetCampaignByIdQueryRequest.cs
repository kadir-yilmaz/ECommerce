using MediatR;

namespace ECommerce.Application.Features.Queries.Campaigns.GetCampaignById
{
    public class GetCampaignByIdQueryRequest : IRequest<GetCampaignByIdQueryResponse>
    {
        public string Id { get; set; }
    }
}
