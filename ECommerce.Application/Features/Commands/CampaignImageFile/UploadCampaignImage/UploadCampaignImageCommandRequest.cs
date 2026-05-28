using Microsoft.AspNetCore.Http;
using MediatR;

namespace ECommerce.Application.Features.Commands.CampaignImageFile.UploadCampaignImage
{
    public class UploadCampaignImageCommandRequest : IRequest<UploadCampaignImageCommandResponse>
    {
        public IFormFileCollection Files { get; set; }
    }
}
