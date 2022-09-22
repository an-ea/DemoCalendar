using AutoMapper;
using Calendar.Application.Dto;
using Calendar.Application.Queries;
using Calendar.Domain;
using Calendar.Domain.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace Calendar.Application.UnitTests.Queries;

public class FindEventsByPeriodQueryHandlerTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(10)]
    public async Task Handle_DifferentPeriods_DifferentResults(int eventCount)
    {
        const int userId = 1;
        var events = Enumerable.Range(0, eventCount).Select(i =>
            {
                var eventMock = new Mock<ICalendarEvent>();
                eventMock.SetupGet(m => m.UserId).Returns(userId);
                eventMock.SetupGet(m => m.Id).Returns(i);
                eventMock.SetupGet(m => m.Subject).Returns($"{nameof(ICalendarEvent.Subject)} {i}");
                eventMock.SetupGet(m => m.Description).Returns($"{nameof(ICalendarEvent.Description)} {i}");
                eventMock.SetupGet(m => m.Begin).Returns(DateTime.Today.AddHours(i));
                eventMock.SetupGet(m => m.End).Returns(DateTime.Today.AddHours(i + 1));
                return eventMock.Object;
            })
            .ToArray();

        var calendarMock = new Mock<ICalendar>();
        calendarMock.Setup(m => m.FindAsync(It.IsAny<int>(), It.IsAny<DateTimeRange>())).ReturnsAsync(events);
        var mapperMock = new Mock<IMapper>(MockBehavior.Strict);
        mapperMock.Setup(m => m.Map<IEnumerable<EventDto>>(It.IsAny<IEnumerable<ICalendarEvent>>()))
            .Returns((IEnumerable<ICalendarEvent> src) => src.Select(e =>
                    new EventDto(e.Id, e.Subject, e.Description,e.Begin, e.End))
                .ToArray());

        var (begin, end) = eventCount != 0
            ? (events.Min(e => e.Begin), events.Max(e => e.Begin))
            : (DateTime.Today, DateTime.Today.AddHours(1));

        var query = new FindEventsByPeriodQuery { UserId = userId, Begin = begin, End = end };
        var sut = new FindEventsByPeriodQueryHandler(calendarMock.Object, mapperMock.Object);




        var result = await sut.Handle(query, CancellationToken.None);




        result.Should().BeEquivalentTo(events, o => o.Excluding(e => e.UserId));
        calendarMock.Verify(m => m.FindAsync(
            It.Is<int>(id => id == query.UserId),
            It.Is<DateTimeRange>(r => r.Begin == query.Begin && r.End == query.End)), Times.Once);
        mapperMock.Verify(m => m.Map<IEnumerable<EventDto>>(It.IsAny<IEnumerable<ICalendarEvent>>()), Times.Once);
    }

}