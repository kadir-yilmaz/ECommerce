using MediatR;

namespace ECommerce.Application.Features.Commands.Category.ChangeShowcaseCategory
{
    public class ChangeShowcaseCategoryCommandRequest : IRequest<ChangeShowcaseCategoryCommandResponse>
    {
        public string Id { get; set; }
        public bool ShowOnHomepage { get; set; }
    }
}
