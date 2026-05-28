using ECommerce.Application.Repositories.Category;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace ECommerce.Application.Features.Queries.Category.GetByIdCategory
{
    public class GetByIdCategoryQueryHandler : IRequestHandler<GetByIdCategoryQueryRequest, GetByIdCategoryQueryResponse>
    {
        private readonly ICategoryReadRepository _categoryReadRepository;

        public GetByIdCategoryQueryHandler(ICategoryReadRepository categoryReadRepository)
        {
            _categoryReadRepository = categoryReadRepository;
        }

        public async Task<GetByIdCategoryQueryResponse> Handle(GetByIdCategoryQueryRequest request, CancellationToken cancellationToken)
        {
            var category = await _categoryReadRepository.GetByIdAsync(request.Id);
            if (category == null)
                throw new System.Collections.Generic.KeyNotFoundException("Category not found");

            return new GetByIdCategoryQueryResponse
            {
                Id = category.Id.ToString(),
                Name = category.Name,
                ParentCategoryId = category.ParentCategoryId
            };
        }
    }
}
