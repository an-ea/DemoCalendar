using AutoFixture;
using AutoFixture.AutoMoq;
using AutoMapper;
using Calendar.Domain;
using Calendar.Domain.Abstract;
using Calendar.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace Calendar.Infrastructure.UnitTests.Repositories;

public class EventRepositoryTests
{
    private const int UserId = 1;
    private const string UserIdName = "userId";
    private const string EventIdName = "eventId";
    private const string EventName = "event";


    [Fact]
    public async Task FindAsync_ReturnsEvents_ReturnedEventsEqualExpected()
    {
        var (context, expectedEvents) = CreateDbContextAndEvents(3);
        await using var sut = CreateEventRepository(context);
        var range = new DateTimeRange(expectedEvents.Min(e => e.Begin), expectedEvents.Max(e => e.Begin).AddDays(1));



        var events = await sut.FindAsync(UserId, range);



        events.Should().BeEquivalentTo(expectedEvents);
    }

    [Theory]
    [InlineData(int.MinValue)]
    [InlineData(-1)]
    [InlineData(0)]
    public void FindAsync_InvalidId_ArgumentOutOfRangeException(int userId) =>
        EnsureThrowsArgumentOutOfRangeException(r => 
            r.FindAsync(userId, new DateTimeRange(DateTime.Today, DateTime.Today.AddHours(1))), UserIdName);



    [Fact]
    public async Task FindAsync_CorrectParams_ReturnsExpectedEvent()
    {
        var (context, expectedEvents) = CreateDbContextAndEvents(3);
        var expectedEvent = expectedEvents[1];
        await using var sut = CreateEventRepository(context);



        var @event = await sut.FindAsync(expectedEvent.UserId, expectedEvent.Id);



        @event.Should().BeEquivalentTo(expectedEvent);
    }


    [Theory]
    [InlineData(int.MinValue, int.MinValue, UserIdName)]
    [InlineData(int.MinValue, 1, UserIdName)]
    [InlineData(1, int.MinValue, EventIdName)]
    [InlineData(0, 0, UserIdName)]
    [InlineData(0, 1, UserIdName)]
    [InlineData(1, 0, EventIdName)]
    [InlineData(-1, 1, UserIdName)]
    [InlineData(1, -1, EventIdName)]
    public void FindAsync_InvalidIds_ArgumentOutOfRangeException(int userId, int eventId, string parameterName) =>
        EnsureThrowsArgumentOutOfRangeException(r => r.FindAsync(userId, eventId), parameterName);


    [Fact]
    public async Task FindAsync_OneEventSpecification_ReturnsRightEvent()
    {
        var (context, events) = CreateDbContextAndEvents(3);
        await using var sut = CreateEventRepository(context);
        var expectedEvent = events[1];

        var specificationMock = new Mock<IEventEntitySpecification>(MockBehavior.Strict);
        specificationMock.SetupGet(m => m.IsSatisfiedBy).Returns(e => e.Id == expectedEvent.Id);



        var result = (await sut.FindAsync(specificationMock.Object)).ToArray();



        result.Should().HaveCount(1);
        result[0].Should().BeEquivalentTo(expectedEvent);
        specificationMock.Verify(s => s.IsSatisfiedBy, Times.Once);
    }


    [Fact]
    public void FindAsync_NullReference_ThrowsArgumentNullException() =>
        EnsureThrowsArgumentNullException(r => r.FindAsync(null!), "specification");


    [Fact]
    public async Task CreateAsync_NewEvent_ReturnsNewEventId()
    {
        var newEvent = new NewCalendarEvent(UserId, "TestSubject", "TestDescription", DateTime.Now, DateTime.Now.AddHours(1));

        var mapperMock = new Mock<IMapper>(MockBehavior.Strict);
        mapperMock.Setup(m => m.Map<EventEntity>(It.IsAny<INewCalendarEvent>()))
            .Returns((INewCalendarEvent src) =>
                new EventEntity
                {
                    Subject = src.Subject,
                    Description = src.Description,
                    UserId = src.UserId,
                    Begin = src.Begin,
                    End = src.End
                });

        await using var context = CreateDbContext();
        await using var sut = new EventRepository(context, mapperMock.Object);



        var newEventId = await sut.CreateAsync(newEvent);



        newEventId.Should().Be(1);
        EventsShouldBeEquivalent(context, new[]{newEvent});
    }


    [Fact]
    public void CreateAsync_NullReference_ThrowsArgumentNullException() =>
        EnsureThrowsArgumentNullException(r => r.CreateAsync(null!), EventName);
        


    [Fact]
    public async Task UpdateAsync_ValidEvent_ReturnsTrue()
    {
        var mapperMock = new Mock<IMapper>(MockBehavior.Strict);
        mapperMock.Setup(m => m.Map<EventEntity>(It.IsAny<ICalendarEvent>()))
            .Returns((ICalendarEvent src) =>
                new EventEntity
                {
                    Id = src.Id,
                    Subject = src.Subject,
                    Description = src.Description,
                    UserId = src.UserId,
                    Begin = src.Begin,
                    End = src.End
                });

        var (context, expectedEvents) = CreateDbContextAndEvents(3);
        var middle = expectedEvents.Skip(1).First();
        middle.Subject += "1";
        middle.Description += "2";
        middle.Begin += TimeSpan.FromMinutes(10);
        middle.End += TimeSpan.FromMinutes(20);

        await using var sut = new EventRepository(context, mapperMock.Object);



        var result = await sut.UpdateAsync(middle);



        result.Should().BeTrue();
        EventsShouldBeEquivalent(context, expectedEvents);
    }


    [Fact]
    public void UpdateAsync_NullReference_ThrowsArgumentNullException() =>
        EnsureThrowsArgumentNullException(c => c.UpdateAsync(null!), EventName);



    [Fact]
    public async Task DeleteAsync_ValidEvent_ReturnsTrue()
    {
        var (context, events) = CreateDbContextAndEvents(3);
        var middle = events[1];
        await using var repository = CreateEventRepository(context);



        var result = await repository.DeleteAsync(UserId, middle.Id);



        result.Should().BeTrue();
        var existEvents = new[] { events[0], events[^1] };
        EventsShouldBeEquivalent(context, existEvents);
    }


    [Theory]
    [InlineData(int.MinValue, 1, UserIdName)]
    [InlineData(-1, 1, UserIdName)]
    [InlineData(0, 1, UserIdName)]
    [InlineData(1, int.MinValue, EventIdName)]
    [InlineData(1, -1, EventIdName)]
    [InlineData(1, 0, EventIdName)]
    public void DeleteAsync_InvalidId_ThrowsArgumentOutOfRangeException(int userId, int eventId, string name) =>
        EnsureThrowsArgumentOutOfRangeException(c => c.DeleteAsync(userId, eventId), name);


    private static void EventsShouldBeEquivalent<T>(CalendarDbContext context, IEnumerable<T> expectedEvents)
    {
        var eventFromDb = context.Events.ToArray();
        eventFromDb.Should().BeEquivalentTo(expectedEvents, opt => opt.ExcludingMissingMembers());
    }


    private static void EnsureThrowsArgumentNullException(Func<EventRepository, Task> runMethod, string partOfMessage) =>
        EnsureThrowsException<ArgumentNullException>(runMethod, partOfMessage);

    private static void EnsureThrowsArgumentOutOfRangeException(Func<EventRepository, Task> runMethod, string partOfMessage) =>
        EnsureThrowsException<ArgumentOutOfRangeException>(runMethod, partOfMessage);

    private static void EnsureThrowsException<TException>(Func<EventRepository, Task> runMethod, string partOfMessage) where TException : Exception
    {
        var repository = CreateEventRepository(CreateDbContext());


        var act = () => runMethod(repository).Wait();


        act.Should().Throw<TException>().Where(e => e.Message.Contains(partOfMessage));
    }

    private static EventRepository CreateEventRepository(CalendarDbContext context) =>
        new(context, Mock.Of<IMapper>(MockBehavior.Strict));


    private static CalendarDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<CalendarDbContext>()
            .UseInMemoryDatabase("TestCalendarDb")
            .Options;

        var context = new CalendarDbContext(options);
        context.Database.EnsureDeleted();
        return context;
    }

    private static (CalendarDbContext, IReadOnlyList<EventEntity>) CreateDbContextAndEvents(int generateEvents)
    {
        var fixture = new Fixture().Customize(new AutoMoqCustomization { ConfigureMembers = true });
        var events = Enumerable.Range(1, generateEvents).Select(i => 
            fixture.Build<EventEntity>()
                .With(p => p.UserId, UserId)
                .With(p => p.Id, i)
                .Create())
            .ToArray();

        var context = CreateDbContext();
        context.Events.AddRange(events);
        context.SaveChanges();

        return (context, events);
    }

}