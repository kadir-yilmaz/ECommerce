using MediatR;

namespace ECommerce.Application.Features.Commands.CampaignImageFile.RemoveCampaignImage
{
    public class RemoveCampaignImageCommandRequest : IRequest<RemoveCampaignImageCommandResponse>
    {
        public string Id { get; set; }
    }
}
