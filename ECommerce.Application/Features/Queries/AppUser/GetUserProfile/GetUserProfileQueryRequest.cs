using MediatR;

namespace ECommerce.Application.Features.Queries.AppUser.GetUserProfile
{
    public class GetUserProfileQueryRequest : IRequest<GetUserProfileQueryResponse>
    {
        public string UserId { get; set; } = string.Empty;
    }
}
