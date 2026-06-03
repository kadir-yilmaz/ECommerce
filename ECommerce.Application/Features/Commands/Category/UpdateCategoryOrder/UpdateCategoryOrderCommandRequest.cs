using MediatR;
using System.Collections.Generic;

namespace ECommerce.Application.Features.Commands.Category.UpdateCategoryOrder
{
    public class CategoryOrderDto
    {
        public string Id { get; set; }
        public int Order { get; set; }
    }

    public class UpdateCategoryOrderCommandRequest : IRequest<UpdateCategoryOrderCommandResponse>
    {
        public List<CategoryOrderDto> Orders { get; set; }
    }
}
