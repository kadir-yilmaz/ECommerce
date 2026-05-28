using ECommerce.Application.Repositories.Category;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace ECommerce.Application.Features.Commands.Category.UpdateCategory
{
    public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommandRequest, UpdateCategoryCommandResponse>
    {
        private readonly ICategoryReadRepository _categoryReadRepository;
        private readonly ICategoryWriteRepository _categoryWriteRepository;

        public UpdateCategoryCommandHandler(ICategoryReadRepository categoryReadRepository, ICategoryWriteRepository categoryWriteRepository)
        {
            _categoryReadRepository = categoryReadRepository;
            _categoryWriteRepository = categoryWriteRepository;
        }

        public async Task<UpdateCategoryCommandResponse> Handle(UpdateCategoryCommandRequest request, CancellationToken cancellationToken)
        {
            Domain.Entities.Category category = await _categoryReadRepository.GetByIdAsync(request.Id);
            if (category == null)
                throw new System.Collections.Generic.KeyNotFoundException("Category not found");

            category.Name = request.Name;
            category.ParentCategoryId = request.ParentCategoryId;
            await _categoryWriteRepository.SaveAsync();
            return new UpdateCategoryCommandResponse();
        }
    }
}
