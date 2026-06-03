using ECommerce.Application.Repositories.Category;
using MediatR;

namespace ECommerce.Application.Features.Commands.Category.UpdateCategoryOrder
{
    public class UpdateCategoryOrderCommandHandler : IRequestHandler<UpdateCategoryOrderCommandRequest, UpdateCategoryOrderCommandResponse>
    {
        readonly ICategoryReadRepository _categoryReadRepository;
        readonly ICategoryWriteRepository _categoryWriteRepository;

        public UpdateCategoryOrderCommandHandler(ICategoryReadRepository categoryReadRepository, ICategoryWriteRepository categoryWriteRepository)
        {
            _categoryReadRepository = categoryReadRepository;
            _categoryWriteRepository = categoryWriteRepository;
        }

        public async Task<UpdateCategoryOrderCommandResponse> Handle(UpdateCategoryOrderCommandRequest request, CancellationToken cancellationToken)
        {
            if (request.Orders == null || !request.Orders.Any())
                return new();

            foreach (var orderDto in request.Orders)
            {
                var category = await _categoryReadRepository.GetByIdAsync(orderDto.Id);
                if (category != null)
                {
                    category.HomepageOrder = orderDto.Order;
                }
            }

            await _categoryWriteRepository.SaveAsync();
            return new();
        }
    }
}
