using MediatR;

namespace ECommerce.Application.Features.Queries.CampaignImageFile.GetCampaignImages
{
    public class GetCampaignImagesQueryRequest : IRequest<List<GetCampaignImagesQueryResponse>>
    {
    }
}
