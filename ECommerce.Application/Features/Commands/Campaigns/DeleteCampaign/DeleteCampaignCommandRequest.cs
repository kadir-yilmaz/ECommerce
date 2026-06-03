using MediatR;

namespace ECommerce.Application.Features.Commands.Campaigns.DeleteCampaign
{
    public class DeleteCampaignCommandRequest : IRequest<DeleteCampaignCommandResponse>
    {
        public string Id { get; set; }
    }
}
