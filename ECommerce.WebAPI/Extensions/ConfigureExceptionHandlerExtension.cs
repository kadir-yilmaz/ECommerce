using ECommerce.Application.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using System.Net;
using System.Net.Mime;
using System.Text.Json;

namespace ECommerce.WebAPI.Extensions
{
    static public class ConfigureExceptionHandlerExtension
    {
        public static void ConfigureExceptionHandler<T>(this WebApplication application, ILogger<T> logger)
        {
            application.UseExceptionHandler(builder =>
            {
                builder.Run(async context =>
                {
                    context.Response.ContentType = MediaTypeNames.Application.Json;

                    var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                    if (contextFeature != null)
                    {
                        context.Response.StatusCode = contextFeature.Error switch
                        {
                            AuthenticationErrorException => (int)HttpStatusCode.Unauthorized,
                            NotFoundUserException => (int)HttpStatusCode.NotFound,
                            _ => (int)HttpStatusCode.InternalServerError
                        };

                        logger.LogError(contextFeature.Error, "Unhandled exception at {Path}", context.Request.Path);

                        await context.Response.WriteAsync(JsonSerializer.Serialize(new
                        {
                            StatusCode = context.Response.StatusCode,
                            Message = contextFeature.Error.Message,
                            Title = "Hata alındı!"
                        }));
                    }
                });
            });
        }
    }
}
