using AutoFixture;
using AutoFixture.AutoMoq;
using AutoMapper;
using Calendar.Application.Dto;
using Calendar.Application.Queries;
using Calendar.Domain.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace Calendar.Application.UnitTests.Queries;

public class FindEventByIdQueryHandlerTests
{
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Handle_IdQuery_DifferentResults(bool returnNullEvent)
    {
        var fixture = new Fixture().Customize(new AutoMoqCustomization { ConfigureMembers = true });
        var @event = returnNullEvent? null : fixture.Create<ICalendarEvent>();

        var calendarMock = new Mock<ICalendar>();
        calendarMock.Setup(m => m.FindAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(@event);
        var mapperMock = new Mock<IMapper>(MockBehavior.Strict);
        mapperMock.Setup(m => m.Map<EventDto>(It.IsAny<ICalendarEvent>()))
            .Returns((ICalendarEvent src) =>
                new EventDto(src.Id, src.Subject, src.Description, src.Begin, src.End));

        var query = new FindEventByIdQuery { Id = DateTime.Now.Millisecond + 1, UserId = 1 };
        var sut = new FindEventByIdQueryHandler(calendarMock.Object, mapperMock.Object);





        var result = await sut.Handle(query, CancellationToken.None);



        

        calendarMock.Verify(m => m.FindAsync(
            It.Is<int>(id => id == query.UserId), 
            It.Is<int>(id => id == query.Id)), Times.Once);
        
        mapperMock.Verify(m => m.Map<EventDto>(It.Is<ICalendarEvent>(e => e == @event)), 
            returnNullEvent ? Times.Never : Times.Once);

        if (returnNullEvent)
            result.Should().BeNull();
        else
            result.Should().BeEquivalentTo(@event, o => o.Excluding(e => e!.UserId));
    }
}