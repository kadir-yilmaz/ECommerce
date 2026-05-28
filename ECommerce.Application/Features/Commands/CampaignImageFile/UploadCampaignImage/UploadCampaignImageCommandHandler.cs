using ECommerce.Application.Abstractions.Storage;
using ECommerce.Application.Repositories;
using MediatR;

namespace ECommerce.Application.Features.Commands.CampaignImageFile.UploadCampaignImage
{
    public class UploadCampaignImageCommandHandler : IRequestHandler<UploadCampaignImageCommandRequest, UploadCampaignImageCommandResponse>
    {
        readonly IStorageService _storageService;
        readonly ICampaignImageFileWriteRepository _campaignImageFileWriteRepository;

        public UploadCampaignImageCommandHandler(IStorageService storageService, ICampaignImageFileWriteRepository campaignImageFileWriteRepository)
        {
            _storageService = storageService;
            _campaignImageFileWriteRepository = campaignImageFileWriteRepository;
        }

        public async Task<UploadCampaignImageCommandResponse> Handle(UploadCampaignImageCommandRequest request, CancellationToken cancellationToken)
        {
            List<(string fileName, string pathOrContainerName)> result = await _storageService.UploadAsync("campaign-images", request.Files);

            await _campaignImageFileWriteRepository.AddRangeAsync(result.Select(r => new Domain.Entities.CampaignImageFile
            {
                FileName = r.fileName,
                Path = r.pathOrContainerName,
                Storage = _storageService.StorageName,
                Showcase = false
            }).ToList());

            await _campaignImageFileWriteRepository.SaveAsync();

            return new();
        }
    }
}
