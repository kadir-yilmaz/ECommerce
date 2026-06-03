using MediatR;
using System;

namespace ECommerce.Application.Features.Commands.Category.CreateCategory
{
    public class CreateCategoryCommandRequest : IRequest<CreateCategoryCommandResponse>
    {
        public string Name { get; set; }
        public Guid? ParentCategoryId { get; set; }
        public bool ShowOnHomepage { get; set; }
        public int HomepageOrder { get; set; }
    }
}
