using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication;

namespace WebClient.Infrastructure;

/// <summary>
/// Represents a handler that adds authorization headers.
/// </summary>
public class HttpClientAuthorizationDelegatingHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    /// <summary>
    /// Initializes a new instance of the <see cref="HttpClientAuthorizationDelegatingHandler" /> class.
    /// </summary>
    /// <param name="httpContextAccessor">Current HTTP context.</param>
    /// <exception cref="ArgumentNullException"></exception>
    public HttpClientAuthorizationDelegatingHandler(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var authorizationHeader = _httpContextAccessor.HttpContext!.Request.Headers["Authorization"];

        if (!string.IsNullOrEmpty(authorizationHeader))
        {
            request.Headers.Add("Authorization", new List<string> { authorizationHeader });
        }

        var token = await _httpContextAccessor.HttpContext.GetTokenAsync("access_token");

        if (token != null)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
