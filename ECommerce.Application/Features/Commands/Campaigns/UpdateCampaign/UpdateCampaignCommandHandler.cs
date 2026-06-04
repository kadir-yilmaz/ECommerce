using ECommerce.Application.Repositories.Campaign;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace ECommerce.Application.Features.Commands.Campaigns.UpdateCampaign
{
    public class UpdateCampaignCommandHandler : IRequestHandler<UpdateCampaignCommandRequest, UpdateCampaignCommandResponse>
    {
        private readonly ICampaignWriteRepository _campaignWriteRepository;
        private readonly ICampaignReadRepository _campaignReadRepository;

        public UpdateCampaignCommandHandler(ICampaignWriteRepository campaignWriteRepository, ICampaignReadRepository campaignReadRepository)
        {
            _campaignWriteRepository = campaignWriteRepository;
            _campaignReadRepository = campaignReadRepository;
        }

        public async Task<UpdateCampaignCommandResponse> Handle(UpdateCampaignCommandRequest request, CancellationToken cancellationToken)
        {
            var campaign = await _campaignReadRepository.GetByIdAsync(request.Id);
            if (campaign == null)
            {
                return new UpdateCampaignCommandResponse { Success = false, Message = "Kampanya bulunamadı." };
            }

            campaign.Name = request.Name;
            campaign.Description = request.Description;
            campaign.RuleType = request.RuleType;
            campaign.MinAmount = request.MinAmount;
            campaign.DiscountRate = request.DiscountRate;
            campaign.MinQuantity = request.MinQuantity;
            campaign.FreeQuantity = request.FreeQuantity;
            campaign.ProductId = request.ProductId;
            campaign.CategoryId = request.CategoryId;
            campaign.Brand = request.Brand;
            campaign.EndDate = request.EndDate;
            campaign.IsActive = request.IsActive;

            _campaignWriteRepository.Update(campaign);
            await _campaignWriteRepository.SaveAsync();

            return new UpdateCampaignCommandResponse { Success = true, Message = "Kampanya güncellendi." };
        }
    }
}
