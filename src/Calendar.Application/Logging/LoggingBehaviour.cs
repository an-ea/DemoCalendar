using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Calendar.Application.Logging;

/// <summary>
/// Represents a behaviour used for message logging.
/// </summary>
/// <typeparam name="TRequest">Type of request message.</typeparam>
/// <typeparam name="TResponse">Type of response message.</typeparam>
public class LoggingBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<LoggingBehaviour<TRequest, TResponse>> _logger;

    public LoggingBehaviour(ILogger<LoggingBehaviour<TRequest, TResponse>> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
    {
        var requestName = typeof(TRequest).Name;

        var properties = request.GetType().GetProperties();
        var namesAndValues = properties.Select(p => (p.Name, Value: p.GetValue(request, null)));
        var parameters = string.Join(", ", namesAndValues.Select(x => $"{x.Name} = {x.Value}"));

        _logger.LogInformation("Start handling {request}. Params: {params}.", requestName, parameters);
        
        var stopwatch = Stopwatch.StartNew();

        var response = await next();

        stopwatch.Stop();

        _logger.LogInformation("Finish handling {request}. Elapsed time: {time}.", requestName, stopwatch.Elapsed);
        return response;
    }
}