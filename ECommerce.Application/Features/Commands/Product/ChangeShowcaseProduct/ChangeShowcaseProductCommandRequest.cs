using MediatR;

namespace ECommerce.Application.Features.Commands.Product.ChangeShowcaseProduct
{
    public class ChangeShowcaseProductCommandRequest : IRequest<ChangeShowcaseProductCommandResponse>
    {
        public string Id { get; set; }
        public bool ShowOnHomepage { get; set; }
    }
}
