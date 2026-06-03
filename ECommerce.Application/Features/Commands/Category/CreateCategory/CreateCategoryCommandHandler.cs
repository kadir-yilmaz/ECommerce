using ECommerce.Application.Repositories.Category;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace ECommerce.Application.Features.Commands.Category.CreateCategory
{
    public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommandRequest, CreateCategoryCommandResponse>
    {
        private readonly ICategoryWriteRepository _categoryWriteRepository;

        public CreateCategoryCommandHandler(ICategoryWriteRepository categoryWriteRepository)
        {
            _categoryWriteRepository = categoryWriteRepository;
        }

        public async Task<CreateCategoryCommandResponse> Handle(CreateCategoryCommandRequest request, CancellationToken cancellationToken)
        {
            await _categoryWriteRepository.AddAsync(new Domain.Entities.Category()
            {
                Name = request.Name,
                ParentCategoryId = request.ParentCategoryId,
                ShowOnHomepage = request.ShowOnHomepage,
                HomepageOrder = request.HomepageOrder
            });
            await _categoryWriteRepository.SaveAsync();
            return new CreateCategoryCommandResponse();
        }
    }
}
