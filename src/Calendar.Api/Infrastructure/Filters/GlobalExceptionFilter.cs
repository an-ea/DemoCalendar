using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Calendar.Api.Infrastructure.Filters;

/// <summary>
/// Represents a global exception filter.
/// </summary>
internal class GlobalExceptionFilter : IExceptionFilter
{
    private readonly ILogger<GlobalExceptionFilter> _logger;

    public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public void OnException(ExceptionContext context)
    {
        _logger.LogError(new EventId(context.Exception.HResult),
            context.Exception,
            context.Exception.Message);

        context.Result = new InternalServerErrorObjectResult("An error occurred");
        context.HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        
        context.ExceptionHandled = true;
    }


    private class InternalServerErrorObjectResult : ObjectResult
    {
        public InternalServerErrorObjectResult(object error) : base(error)
        {
            StatusCode = StatusCodes.Status500InternalServerError;
        }
    }
}
