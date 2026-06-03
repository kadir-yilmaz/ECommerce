using ECommerce.Application.Repositories.Campaign;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace ECommerce.Application.Features.Commands.Campaigns.DeleteCampaign
{
    public class DeleteCampaignCommandHandler : IRequestHandler<DeleteCampaignCommandRequest, DeleteCampaignCommandResponse>
    {
        private readonly ICampaignWriteRepository _campaignWriteRepository;

        public DeleteCampaignCommandHandler(ICampaignWriteRepository campaignWriteRepository)
        {
            _campaignWriteRepository = campaignWriteRepository;
        }

        public async Task<DeleteCampaignCommandResponse> Handle(DeleteCampaignCommandRequest request, CancellationToken cancellationToken)
        {
            await _campaignWriteRepository.RemoveAsync(request.Id);
            await _campaignWriteRepository.SaveAsync();

            return new DeleteCampaignCommandResponse
            {
                Success = true,
                Message = "Kampanya başarıyla silindi."
            };
        }
    }
}
