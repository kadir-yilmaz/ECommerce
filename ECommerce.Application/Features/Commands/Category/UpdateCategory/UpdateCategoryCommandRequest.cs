using MediatR;
using System;

namespace ECommerce.Application.Features.Commands.Category.UpdateCategory
{
    public class UpdateCategoryCommandRequest : IRequest<UpdateCategoryCommandResponse>
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public Guid? ParentCategoryId { get; set; }
        public bool ShowOnHomepage { get; set; }
        public int HomepageOrder { get; set; }
    }
}
