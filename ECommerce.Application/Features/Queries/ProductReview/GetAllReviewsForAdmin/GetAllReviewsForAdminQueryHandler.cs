using ECommerce.Application.Repositories.ProductReview;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ECommerce.Application.Features.Queries.ProductReview.GetAllReviewsForAdmin
{
    public class GetAllReviewsForAdminQueryHandler : IRequestHandler<GetAllReviewsForAdminQueryRequest, GetAllReviewsForAdminQueryResponse>
    {
        private readonly IProductReviewReadRepository _reviewReadRepository;

        public GetAllReviewsForAdminQueryHandler(IProductReviewReadRepository reviewReadRepository)
        {
            _reviewReadRepository = reviewReadRepository;
        }

        public async Task<GetAllReviewsForAdminQueryResponse> Handle(GetAllReviewsForAdminQueryRequest request, CancellationToken cancellationToken)
        {
            var query = _reviewReadRepository.GetAll(false)
                .Where(r => !r.IsDeleted);

            if (request.Status.HasValue)
                query = query.Where(r => r.Status == request.Status.Value);

            var totalCount = await query.CountAsync(cancellationToken);

            var reviews = await query
                .Include(r => r.User)
                .Include(r => r.Product)
                .OrderByDescending(r => r.CreatedDate)
                .Skip(request.Page * request.Size)
                .Take(request.Size)
                .Select(r => new
                {
                    r.Id,
                    r.ProductId,
                    ProductName = r.Product != null ? r.Product.Name : "",
                    r.UserId,
                    UserName = r.User != null ? r.User.NameSurname : "Anonim",
                    UserEmail = r.User != null ? r.User.Email : "",
                    r.Rating,
                    r.Comment,
                    r.Status,
                    r.HasProfanity,
                    r.HasPriceInfo,
                    r.AdminNote,
                    r.CreatedDate,
                    r.UpdatedDate
                })
                .ToListAsync(cancellationToken);

            return new GetAllReviewsForAdminQueryResponse
            {
                Reviews = reviews,
                TotalCount = totalCount
            };
        }
    }
}
