using ECommerce.Application.Repositories.Category;
using MediatR;

namespace ECommerce.Application.Features.Commands.Category.ChangeShowcaseCategory
{
    public class ChangeShowcaseCategoryCommandHandler : IRequestHandler<ChangeShowcaseCategoryCommandRequest, ChangeShowcaseCategoryCommandResponse>
    {
        readonly ICategoryReadRepository _categoryReadRepository;
        readonly ICategoryWriteRepository _categoryWriteRepository;

        public ChangeShowcaseCategoryCommandHandler(ICategoryReadRepository categoryReadRepository, ICategoryWriteRepository categoryWriteRepository)
        {
            _categoryReadRepository = categoryReadRepository;
            _categoryWriteRepository = categoryWriteRepository;
        }

        public async Task<ChangeShowcaseCategoryCommandResponse> Handle(ChangeShowcaseCategoryCommandRequest request, CancellationToken cancellationToken)
        {
            var category = await _categoryReadRepository.GetByIdAsync(request.Id);
            if (category == null)
                throw new System.Collections.Generic.KeyNotFoundException("Category not found");

            category.ShowOnHomepage = request.ShowOnHomepage;
            
            // Auto increment order if adding to showcase
            if (request.ShowOnHomepage) {
                var maxOrder = _categoryReadRepository.GetAll(false).Max(c => (int?)c.HomepageOrder) ?? 0;
                category.HomepageOrder = maxOrder + 1;
            }

            await _categoryWriteRepository.SaveAsync();
            return new();
        }
    }
}
