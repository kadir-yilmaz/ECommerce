using ECommerce.Application.Repositories.Campaign;
using ECommerce.Domain.Entities;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace ECommerce.Application.Features.Commands.Campaigns.CreateCampaign
{
    public class CreateCampaignCommandHandler : IRequestHandler<CreateCampaignCommandRequest, CreateCampaignCommandResponse>
    {
        private readonly ICampaignWriteRepository _campaignWriteRepository;

        public CreateCampaignCommandHandler(ICampaignWriteRepository campaignWriteRepository)
        {
            _campaignWriteRepository = campaignWriteRepository;
        }

        public async Task<CreateCampaignCommandResponse> Handle(CreateCampaignCommandRequest request, CancellationToken cancellationToken)
        {
            var campaign = new Campaign
            {
                Name = request.Name,
                Description = request.Description,
                RuleType = request.RuleType,
                MinAmount = request.MinAmount,
                DiscountRate = request.DiscountRate,
                MinQuantity = request.MinQuantity,
                FreeQuantity = request.FreeQuantity,
                ProductId = request.ProductId,
                CategoryId = request.CategoryId,
                Brand = request.Brand,
                EndDate = request.EndDate,
                IsActive = request.IsActive
            };

            await _campaignWriteRepository.AddAsync(campaign);
            await _campaignWriteRepository.SaveAsync();

            return new CreateCampaignCommandResponse
            {
                Success = true,
                Message = "Kampanya başarıyla oluşturuldu."
            };
        }
    }
}
