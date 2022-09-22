using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using Calendar.Domain.Abstract;
using Calendar.Domain.Specifications;
using FluentAssertions;
using Moq;
using Xunit;

namespace Calendar.Domain.UnitTests;

public class CalendarTests
{
    private const string UserIdName = "userId";
    private const string EventIdName = "eventId";
    private const string EventName = "event";
    private static readonly Random Random = new(DateTime.Now.Millisecond);

    [Fact]
    public async Task FindAsync_ValidDateTimeRange_ReturnsExpectedEvents()
    {
        var expectedEvents = new List<ICalendarEvent>();
        var repositoryMock = new Mock<IEventRepository>(MockBehavior.Strict);
        repositoryMock.Setup(m => m.FindAsync(It.IsAny<int>(), It.IsAny<DateTimeRange>())).ReturnsAsync(expectedEvents);
        var userId = CreateRandomId();
        var range = new DateTimeRange(DateTime.Today, DateTime.Today.AddHours(1));
        var sut = new Calendar(repositoryMock.Object);



        var events = await sut.FindAsync(userId, range);



        events.Should().BeSameAs(expectedEvents);
        repositoryMock.Verify(m => m.FindAsync(
                It.Is<int>(id => id == userId),
                It.Is<DateTimeRange>(r => r.Equals(range))),
            Times.Once);
    }

    [Theory]
    [InlineData(int.MinValue)]
    [InlineData(-1)]
    [InlineData(0)]
    public void FindAsync_InvalidParameter_ThrowsArgumentOutOfRangeException(int userId) =>
        EnsureThrowsArgumentOutOfRangeException(c => c.FindAsync(userId, new DateTimeRange(DateTime.Today, DateTime.Today.AddHours(1))), UserIdName);
    


    [Fact]
    public async Task FindAsync_ValidId_ReturnsExpectedEvent()
    {
        var expectedEvent = Create<ICalendarEvent>();
        var repositoryMock = new Mock<IEventRepository>(MockBehavior.Strict);
        repositoryMock.Setup(m => m.FindAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(expectedEvent);
        var userId = CreateRandomId();
        var eventId = CreateRandomId();
        var sut = new Calendar(repositoryMock.Object);



        var @event = await sut.FindAsync(userId, eventId);



        @event.Should().BeSameAs(expectedEvent);
        repositoryMock.Verify(m => m.FindAsync(
                It.Is<int>(id => id == userId),
                It.Is<int>(id => id == eventId)),
            Times.Once);
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
        EnsureThrowsArgumentOutOfRangeException(c => c.FindAsync(userId, eventId), parameterName);
    

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task CreateAsync_ValidEvent_DifferentResults(bool eventAlreadyExists)
    {
        var expectedEvent = Create<INewCalendarEvent>();
        var expectedEventId = CreateRandomId();
        var repositoryMock = new Mock<IEventRepository>(MockBehavior.Strict);
        repositoryMock.Setup(m => m.CreateAsync(It.IsAny<INewCalendarEvent>())).ReturnsAsync(expectedEventId);
        repositoryMock.Setup(m => m.AnyAsync(It.IsAny<IEventEntitySpecification>())).ReturnsAsync(eventAlreadyExists);
        var sut = new Calendar(repositoryMock.Object);



        var result = await sut.CreateAsync(expectedEvent);



        result.AlreadyExists.Should().Be(eventAlreadyExists);
        result.Id.Should().Be(eventAlreadyExists ? 0 : expectedEventId);
        repositoryMock.Verify(m => m.AnyAsync(It.Is<IEventEntitySpecification>(s => s is EqualEventSpecification)), Times.Once);
        if (!eventAlreadyExists)
            repositoryMock.Verify(m => m.CreateAsync(It.Is<INewCalendarEvent>(e => e == expectedEvent)), Times.Once);
    }


    [Fact]
    public void CreateAsync_NullReference_ThrowsArgumentNullException() =>
        EnsureThrowsArgumentNullException(c => c.CreateAsync(null!), EventName);


    [Theory]
    [InlineData(ResultOfEventUpdating.Success)]
    [InlineData(ResultOfEventUpdating.NotFound)]
    [InlineData(ResultOfEventUpdating.AlreadyExists)]
    public async Task UpdateAsync_ValidEvent_DifferentResults(ResultOfEventUpdating expectedResult)
    {
        var isSuccess = expectedResult == ResultOfEventUpdating.Success;
        var updatedEvent = Create<ICalendarEvent>();
        var repositoryMock = new Mock<IEventRepository>(MockBehavior.Strict);
        repositoryMock.Setup(m => m.UpdateAsync(It.IsAny<ICalendarEvent>())).ReturnsAsync(isSuccess);
        repositoryMock.Setup(m => m.AnyAsync(It.IsAny<IEventEntitySpecification>()))
            .ReturnsAsync(expectedResult == ResultOfEventUpdating.AlreadyExists);
        var sut = new Calendar(repositoryMock.Object);

        
        var result = await sut.UpdateAsync(updatedEvent);


        result.Should().Be(expectedResult);

        if (isSuccess)
            repositoryMock.Verify(m => m.UpdateAsync(It.Is<ICalendarEvent>(e => e == updatedEvent)), Times.Once);
    }


    [Fact]
    public void UpdateAsync_NullReference_ThrowsArgumentNullException() =>
        EnsureThrowsArgumentNullException(c => c.UpdateAsync(null!), EventName);
    

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task DeleteAsync_DifferentData_DifferentResults(bool expectedResult)
    {
        var repositoryMock = new Mock<IEventRepository>(MockBehavior.Strict);
        repositoryMock.Setup(m => m.DeleteAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(expectedResult);
        var userId = CreateRandomId();
        var eventId = CreateRandomId();
        var sut = new Calendar(repositoryMock.Object);


        var result = await sut.DeleteAsync(userId, eventId);


        result.Should().Be(expectedResult);
        repositoryMock.Verify(m => m.DeleteAsync(It.Is<int>(id => id == userId), It.Is<int>(id => id == eventId)), Times.Once);
    }


    [Theory]
    [InlineData(int.MinValue, 1, UserIdName)]
    [InlineData(-1, 1, UserIdName)]
    [InlineData(0, 1, UserIdName)]
    [InlineData(1, int.MinValue, EventIdName)]
    [InlineData(1, -1, EventIdName)]
    [InlineData(1, 0, EventIdName)]
    public void DeleteAsync_InvalidParameter_ArgumentOutOfRangeException(int userId, int eventId, string name) =>
        EnsureThrowsArgumentOutOfRangeException(c => c.DeleteAsync(userId, eventId), name);


    private static int CreateRandomId() => Random.Next(1, 1000);

    private static T Create<T>() =>
        new Fixture().Customize(new AutoMoqCustomization { ConfigureMembers = true }).Create<T>();

    private static void EnsureThrowsArgumentNullException(Func<ICalendar, Task> runMethod, string partOfMessage) =>
        EnsureThrowsException<ArgumentNullException>(runMethod, partOfMessage);

    private static void EnsureThrowsArgumentOutOfRangeException(Func<ICalendar, Task> runMethod, string partOfMessage) =>
        EnsureThrowsException<ArgumentOutOfRangeException>(runMethod, partOfMessage);

    private static void EnsureThrowsException<TException>(Func<ICalendar, Task> runMethod, string partOfMessage) where TException : Exception
    {
        var sut = new Calendar(Mock.Of<IEventRepository>());


        var act = () => runMethod(sut).Wait();


        act.Should().Throw<TException>().Where(e => e.Message.Contains(partOfMessage));
    }
}