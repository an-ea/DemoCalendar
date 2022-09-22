using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Calendar.Api.Infrastructure.Filters;

/// <summary>
/// Represents a filter attribute that validates model state.
/// </summary>
internal class ValidationFilterAttribute : ActionFilterAttribute
{
    private readonly ILogger<ValidationFilterAttribute> _logger;

    public ValidationFilterAttribute(ILogger<ValidationFilterAttribute> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (context.ModelState.IsValid)
            return;

        var errors = string.Join(", ", context.ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)));

        _logger.LogError("Validation failed. Method: {method}, Arguments: {arguments}, Errors: {errors}", 
            context.ActionDescriptor.DisplayName, context.ActionArguments.Keys, errors);

        context.Result = new BadRequestObjectResult(context.ModelState);
    }
}