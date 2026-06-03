using System;

namespace ECommerce.Application.Features.Queries.Category.GetByIdCategory
{
    public class GetByIdCategoryQueryResponse
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public Guid? ParentCategoryId { get; set; }
        public bool ShowOnHomepage { get; set; }
        public int HomepageOrder { get; set; }
    }
}
