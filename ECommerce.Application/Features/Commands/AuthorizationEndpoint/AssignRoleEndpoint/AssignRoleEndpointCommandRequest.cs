using MediatR;

namespace ECommerce.Application.Features.Commands.AuthorizationEndpoint.AssignRoleEndpoint
{
    public class AssignRoleEndpointCommandRequest : IRequest<AssignRoleEndpointCommandResponse>
    {
        public string[] Roles { get; set; } = Array.Empty<string>();
        public string Code { get; set; } = string.Empty;
        public string Menu { get; set; } = string.Empty;
        public Type? Type { get; set; }
    }
}
