using ECommerce.Application.Abstractions.Services;
using ECommerce.Application.CustomAttributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Routing;
using System.Reflection;

namespace ECommerce.WebAPI.Filters
{
    public class RolePermissionFilter : IAsyncActionFilter
    {
        readonly IUserService _userService;

        public RolePermissionFilter(IUserService userService)
        {
            _userService = userService;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var descriptor = context.ActionDescriptor as ControllerActionDescriptor;
            if (descriptor == null)
            {
                await next();
                return;
            }

            var allowsAnonymous =
                descriptor.MethodInfo.GetCustomAttribute<AllowAnonymousAttribute>() != null ||
                descriptor.ControllerTypeInfo.GetCustomAttribute<AllowAnonymousAttribute>() != null;

            if (allowsAnonymous)
            {
                await next();
                return;
            }

            var name = context.HttpContext.User.Identity?.Name;
            if (!string.IsNullOrEmpty(name))
            {
                var attribute = descriptor.MethodInfo.GetCustomAttribute(typeof(AuthorizeDefinitionAttribute)) as AuthorizeDefinitionAttribute;

                if (attribute != null)
                {
                    var httpAttribute = descriptor.MethodInfo.GetCustomAttribute(typeof(HttpMethodAttribute)) as HttpMethodAttribute;

                    var code = $"{(httpAttribute != null ? httpAttribute.HttpMethods.First() : HttpMethods.Get)}.{attribute.ActionType}.{attribute.Definition.Replace(" ", "")}";

                    var hasRole = await _userService.HasRolePermissionToEndpointAsync(name, code);

                    if (!hasRole)
                    {
                        context.Result = new StatusCodeResult(403);
                        return;
                    }
                }
                else
                {
                    // AuthorizeDefinition attribute yoksa, sadece Admin rolü erişebilir
                    var authorizeAttribute = descriptor.MethodInfo.GetCustomAttribute<AuthorizeAttribute>() 
                        ?? descriptor.ControllerTypeInfo.GetCustomAttribute<AuthorizeAttribute>();
                    
                    if (authorizeAttribute != null && authorizeAttribute.AuthenticationSchemes == "Admin")
                    {
                        var userRoles = await _userService.GetRolesToUserAsync(name);
                        if (!userRoles.Contains("Admin"))
                        {
                            context.Result = new StatusCodeResult(403);
                            return;
                        }
                    }
                }
                
                await next();
            }
            else
                await next();
        }
    }
}
