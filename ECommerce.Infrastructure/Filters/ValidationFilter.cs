using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace ECommerce.Infrastructure.Filters
{
    public class ValidationFilter : IAsyncActionFilter
    {
        readonly ILogger<ValidationFilter> _logger;

        public ValidationFilter(ILogger<ValidationFilter> logger)
        {
            _logger = logger;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.ModelState.IsValid)
            {
                var errors = context.ModelState
                       .Where(x => x.Value.Errors.Any())
                       .ToDictionary(e => e.Key, e => e.Value.Errors.Select(e => e.ErrorMessage))
                       .ToArray();

                _logger.LogWarning(
                    "Validation failed for {Method} {Path}. Fields: {Fields}",
                    context.HttpContext.Request.Method,
                    context.HttpContext.Request.Path,
                    string.Join(", ", errors.Select(x => x.Key)));

                context.Result = new BadRequestObjectResult(errors);
                return;
            }

            await next();
        }
    }
}
