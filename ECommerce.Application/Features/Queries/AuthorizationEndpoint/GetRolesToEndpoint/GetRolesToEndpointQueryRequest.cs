using MediatR;

namespace ECommerce.Application.Features.Queries.AuthorizationEndpoint.GetRolesToEndpoint
{
    public class GetRolesToEndpointQueryRequest : IRequest<GetRolesToEndpointQueryResponse>
    {
        public string Code { get; set; } = string.Empty;
        public string Menu { get; set; } = string.Empty;
    }
}
