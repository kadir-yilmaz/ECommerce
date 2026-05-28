using MediatR;

namespace ECommerce.Application.Features.Commands.CampaignImageFile.UpdateCampaignImageTitle
{
    public class UpdateCampaignImageTitleCommandRequest : IRequest<UpdateCampaignImageTitleCommandResponse>
    {
        public string Id { get; set; }
        public string Title { get; set; }
    }
}
