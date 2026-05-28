using ECommerce.Application.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace ECommerce.Application.Features.Queries.CampaignImageFile.GetCampaignImages
{
    public class GetCampaignImagesQueryHandler : IRequestHandler<GetCampaignImagesQueryRequest, List<GetCampaignImagesQueryResponse>>
    {
        readonly ICampaignImageFileReadRepository _campaignImageFileReadRepository;
        readonly IConfiguration _configuration;

        public GetCampaignImagesQueryHandler(ICampaignImageFileReadRepository campaignImageFileReadRepository, IConfiguration configuration)
        {
            _campaignImageFileReadRepository = campaignImageFileReadRepository;
            _configuration = configuration;
        }

        public async Task<List<GetCampaignImagesQueryResponse>> Handle(GetCampaignImagesQueryRequest request, CancellationToken cancellationToken)
        {
            var images = await _campaignImageFileReadRepository.GetAll(false).ToListAsync();

            return images.Select(i => new GetCampaignImagesQueryResponse
            {
                Id = i.Id.ToString(),
                FileName = i.FileName,
                Path = $"{_configuration["BaseStorageUrl"]}/{i.Path.Replace("\\", "/")}",
                Storage = i.Storage,
                Showcase = i.Showcase,
                Title = i.Title
            }).ToList();
        }
    }
}
