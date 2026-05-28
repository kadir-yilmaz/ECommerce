using ECommerce.Application.Abstractions.Storage;
using ECommerce.Application.Repositories;
using MediatR;

namespace ECommerce.Application.Features.Commands.CampaignImageFile.RemoveCampaignImage
{
    public class RemoveCampaignImageCommandHandler : IRequestHandler<RemoveCampaignImageCommandRequest, RemoveCampaignImageCommandResponse>
    {
        readonly IStorageService _storageService;
        readonly ICampaignImageFileWriteRepository _campaignImageFileWriteRepository;
        readonly ICampaignImageFileReadRepository _campaignImageFileReadRepository;

        public RemoveCampaignImageCommandHandler(IStorageService storageService, ICampaignImageFileWriteRepository campaignImageFileWriteRepository, ICampaignImageFileReadRepository campaignImageFileReadRepository)
        {
            _storageService = storageService;
            _campaignImageFileWriteRepository = campaignImageFileWriteRepository;
            _campaignImageFileReadRepository = campaignImageFileReadRepository;
        }

        public async Task<RemoveCampaignImageCommandResponse> Handle(RemoveCampaignImageCommandRequest request, CancellationToken cancellationToken)
        {
            Domain.Entities.CampaignImageFile? image = await _campaignImageFileReadRepository.GetByIdAsync(request.Id);
            
            if (image != null)
            {
                await _storageService.DeleteAsync("campaign-images", image.FileName);
                await _campaignImageFileWriteRepository.RemoveAsync(image.Id.ToString());
                await _campaignImageFileWriteRepository.SaveAsync();
            }

            return new();
        }
    }
}
