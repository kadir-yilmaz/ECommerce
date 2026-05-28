using ECommerce.Application.Repositories;
using MediatR;

namespace ECommerce.Application.Features.Commands.CampaignImageFile.UpdateCampaignImageTitle
{
    public class UpdateCampaignImageTitleCommandHandler : IRequestHandler<UpdateCampaignImageTitleCommandRequest, UpdateCampaignImageTitleCommandResponse>
    {
        readonly ICampaignImageFileWriteRepository _campaignImageFileWriteRepository;
        readonly ICampaignImageFileReadRepository _campaignImageFileReadRepository;

        public UpdateCampaignImageTitleCommandHandler(ICampaignImageFileWriteRepository campaignImageFileWriteRepository, ICampaignImageFileReadRepository campaignImageFileReadRepository)
        {
            _campaignImageFileWriteRepository = campaignImageFileWriteRepository;
            _campaignImageFileReadRepository = campaignImageFileReadRepository;
        }

        public async Task<UpdateCampaignImageTitleCommandResponse> Handle(UpdateCampaignImageTitleCommandRequest request, CancellationToken cancellationToken)
        {
            Domain.Entities.CampaignImageFile? image = await _campaignImageFileReadRepository.GetByIdAsync(request.Id);
            if (image != null)
            {
                image.Title = request.Title;
                await _campaignImageFileWriteRepository.SaveAsync();
            }
            return new UpdateCampaignImageTitleCommandResponse();
        }
    }
}
