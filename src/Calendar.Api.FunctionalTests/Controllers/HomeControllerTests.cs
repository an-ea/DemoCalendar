using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Calendar.Api.FunctionalTests.Controllers;

public class HomeControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public HomeControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory ?? throw new ArgumentNullException(nameof(factory));
    }

    [Fact]
    public async Task Index_RedirectsToSwagger()
    {
        var sut = _factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });


        var response = await sut.GetAsync(string.Empty);


        response.StatusCode.Should().Be(HttpStatusCode.Redirect);
        response.Headers.Location!.OriginalString.Should().Be("/swagger");
    }
    
}