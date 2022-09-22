using System.Net;
using System.Net.Http.Json;
using Calendar.Api.FunctionalTests.Helpers;
using Calendar.Api.Models;
using Calendar.Domain;
using Calendar.Infrastructure;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Calendar.Api.FunctionalTests.Controllers.V1;

public class EventsControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private const string Uri = "/api/v1/Events";

    private const string ValidSubject = nameof(ValidSubject);
    private const string ValidDescription = nameof(ValidDescription);
    private const string ValidBegin = "2022-02-01 12:00:00";
    private const string ValidEnd   = "2022-02-01 12:30:00";
    private const string DefaultDateTimeString = "0001-01-01 00:00:00";


    public EventsControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder => builder.ConfigureServices(ConfigureServices));
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        services.AddTransient<IAuthenticationSchemeProvider, FakeSchemeProvider>();

        var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<CalendarDbContext>));
        if (descriptor != null) services.Remove(descriptor);

        services.AddDbContext<CalendarDbContext>(opt => opt.UseInMemoryDatabase("TestCalendarDb"));

        var sp = services.BuildServiceProvider();
        using var scope = sp.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<CalendarDbContext>();

        db.Database.EnsureDeleted();
    
        db.Events.AddRange(GetSeedingEvents());
        db.SaveChanges();
    }


    [Theory]
    [InlineData("2022-01-01", "2022-01-02", new[] { 0 })]
    [InlineData("2022-01-02", "2022-01-03", new[] { 1 })]
    [InlineData("2022-01-03", "2022-01-04", new[] { 2 })]
    [InlineData("2022-01-01", "2022-01-03", new[] { 0, 1 })]
    [InlineData("2022-01-02", "2022-01-04", new[] { 1, 2 })]
    [InlineData("2022-01-01", "2022-01-04", new[] { 0, 1, 2 })]
    public async Task Get_Period_ReturnsExpectedEvents(string begin, string end, int[] expectedEventIndexes)
    {
        var events = GetSeedingEvents();
        var uri = CreatePeriodUri(begin, end);
        var sut = _factory.CreateClient();
        


        var resultEvents = await sut.GetFromJsonAsync<IReadOnlyCollection<EventModel>>(uri);



        var expectedEvents = expectedEventIndexes.Select(i => events[i]);
        expectedEvents.Should().BeEquivalentTo(resultEvents, opt => opt.ExcludingMissingMembers());
    }

    [Theory]
    [InlineData("2022-01-01", "2022-01-01")]
    [InlineData("2022-01-01", "12345")]
    [InlineData("12345", "2022-01-01")]
    [InlineData("12345", "12345")]
    [InlineData("", "")]
    public async Task Get_InvalidPeriod_BadRequest(string begin, string end)
    {
        var sut = _factory.CreateClient();
        var uri = CreatePeriodUri(begin, end);


        var response = await sut.GetAsync(uri);


        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }


    [Fact]
    public async Task Get_NoPeriod_BadRequest()
    {
        var sut = _factory.CreateClient();


        var response = await sut.GetAsync(Uri);


        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }


    [Theory]
    [InlineData(int.MinValue, HttpStatusCode.BadRequest)]
    [InlineData(-1, HttpStatusCode.BadRequest)]
    [InlineData(1, HttpStatusCode.OK)]
    [InlineData(2, HttpStatusCode.OK)]
    [InlineData(3, HttpStatusCode.OK)]
    [InlineData(4, HttpStatusCode.NotFound)]
    [InlineData(int.MaxValue, HttpStatusCode.NotFound)]
    public async Task Get_DifferentIds_DifferentResults(int id, HttpStatusCode code)
    {
        var sut = _factory.CreateClient();



        var response = await sut.GetAsync(CreateUri(id));




        response.StatusCode.Should().Be(code);
        if (code != HttpStatusCode.OK)
            return;
        var resultEvent = response.Content.ReadFromJsonAsync<EventModel>();
        var expectedEvent = GetSeedingEvents().Single(e => e.Id == id);
        resultEvent.Should().BeEquivalentTo(expectedEvent, opt => opt.ExcludingMissingMembers());
    }


    [Theory]
    [InlineData(ValidSubject, ValidDescription, ValidBegin           , ValidEnd             , HttpStatusCode.OK)]
    [InlineData(ValidSubject, ValidDescription, ValidBegin           , ValidBegin           , HttpStatusCode.BadRequest)]
    [InlineData(""          , ValidDescription, ValidBegin           , ValidEnd             , HttpStatusCode.BadRequest)]
    [InlineData(ValidSubject, ""              , ValidBegin           , ValidEnd             , HttpStatusCode.BadRequest)]
    [InlineData(ValidSubject, ValidDescription, DefaultDateTimeString, ValidEnd             , HttpStatusCode.BadRequest)]
    [InlineData(ValidSubject, ValidDescription, ValidBegin           , DefaultDateTimeString, HttpStatusCode.BadRequest)]
    [InlineData(""          , ""              , DefaultDateTimeString, DefaultDateTimeString, HttpStatusCode.BadRequest)]
    public async Task Post_DifferentNewEvents_DifferentResults(string subject, string description, string begin, string end, HttpStatusCode expectedCode)
    {
        var newEvent = new NewEventModel(subject, description, DateTime.Parse(begin), DateTime.Parse(end));
        var sut = _factory.CreateClient();



        var response = await sut.PostAsJsonAsync(Uri, newEvent);



        response.StatusCode.Should().Be(expectedCode);
        if (expectedCode != HttpStatusCode.OK)
            return;
        var newEventInfo = await response.Content.ReadFromJsonAsync<NewEventInfoModel>();
        newEventInfo.Id.Should().Be(GetSeedingEvents().Count + 1);
    }


    [Theory]
    [InlineData(1, ValidSubject, ValidDescription, ValidBegin, ValidEnd, HttpStatusCode.NoContent)]

    [InlineData(int.MinValue, ValidSubject, ValidDescription, ValidBegin, ValidEnd, HttpStatusCode.BadRequest)]
    [InlineData(-1          , ValidSubject, ValidDescription, ValidBegin, ValidEnd, HttpStatusCode.BadRequest)]
    [InlineData(0           , ValidSubject, ValidDescription, ValidBegin, ValidEnd, HttpStatusCode.BadRequest)]
    [InlineData(4           , ValidSubject, ValidDescription, ValidBegin, ValidEnd, HttpStatusCode.NotFound)]
    [InlineData(int.MaxValue, ValidSubject, ValidDescription, ValidBegin, ValidEnd, HttpStatusCode.NotFound)]

    [InlineData(1, ValidSubject, ValidDescription, ValidBegin           , ValidBegin           , HttpStatusCode.BadRequest)]
    [InlineData(1, ""          , ValidDescription, ValidBegin           , ValidEnd             , HttpStatusCode.BadRequest)]
    [InlineData(1, ValidSubject, ""              , ValidBegin           , ValidEnd             , HttpStatusCode.BadRequest)]
    [InlineData(1, ValidSubject, ValidDescription, DefaultDateTimeString, ValidEnd             , HttpStatusCode.BadRequest)]
    [InlineData(1, ValidSubject, ValidDescription, ValidBegin           , DefaultDateTimeString, HttpStatusCode.BadRequest)]
    [InlineData(1, ""          , ""              , DefaultDateTimeString, DefaultDateTimeString, HttpStatusCode.BadRequest)]
    public async Task Put_DifferentEvents_DifferentResults(int id, string subject, string description, string begin, string end, HttpStatusCode expectedCode)
    {
        var newEvent = new EventModel(id, subject, description, DateTime.Parse(begin), DateTime.Parse(end));
        var sut = _factory.CreateClient();



        var response = await sut.PutAsJsonAsync(Uri, newEvent);



        response.StatusCode.Should().Be(expectedCode);
    }


    [Theory]
    [InlineData(int.MinValue, HttpStatusCode.BadRequest)]
    [InlineData(-1, HttpStatusCode.BadRequest)]
    [InlineData(0, HttpStatusCode.BadRequest)]
    [InlineData(1, HttpStatusCode.NoContent)]
    [InlineData(2, HttpStatusCode.NoContent)]
    [InlineData(3, HttpStatusCode.NoContent)]
    [InlineData(4, HttpStatusCode.NotFound)]
    [InlineData(int.MaxValue, HttpStatusCode.NotFound)]
    public async Task Delete_DifferentIds_DifferentResults(int id, HttpStatusCode code)
    {
        var sut = _factory.CreateClient();


        var response = await sut.DeleteAsync(CreateUri(id));


        response.StatusCode.Should().Be(code);
    }


    private static string CreatePeriodUri(string begin, string end) =>
        QueryHelpers.AddQueryString(Uri, new Dictionary<string, string?>
        {
            { "begin", begin },
            { "end", end }
        });

    private static string CreateUri(object obj) => $"{Uri}/{obj}";


    public static IReadOnlyList<EventEntity> GetSeedingEvents() =>
        Enumerable.Range(1, 3).Select(i => new EventEntity
        {
            Id = i,
            UserId = 1,
            Subject = $"TestSubject {i}",
            Description = $"TestDescription {i}",
            Begin = new DateTime(2022, 01, i, 12, 00, 00),
            End   = new DateTime(2022, 01, i, 12, 30, 00)
        }).ToArray();

}