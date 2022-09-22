using AutoFixture;
using AutoFixture.AutoMoq;
using AutoMapper;
using Calendar.Application.Commands.UpdateEvent;
using Calendar.Domain;
using Calendar.Domain.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace Calendar.Application.UnitTests.Commands.UpdateEvent;

public class UpdateEventCommandHandlerTests
{
    [Theory]
    [InlineData(UpdateEventCommandResult.Success)]
    [InlineData(UpdateEventCommandResult.NotFound)]
    [InlineData(UpdateEventCommandResult.AlreadyExists)]
    public async Task Handle_UpdateCommand_DifferentResults(UpdateEventCommandResult expectedResult)
    {
        var fixture = new Fixture().Customize(new AutoMoqCustomization { ConfigureMembers = true });
        var command = fixture.Build<UpdateEventCommand>()
            .With(p => p.Begin, DateTime.Today)
            .With(p => p.End, DateTime.Today.AddHours(1))
            .Create();

        var calendarMock = new Mock<ICalendar>();
        calendarMock.Setup(m => m.UpdateAsync(It.IsAny<ICalendarEvent>())).ReturnsAsync((ResultOfEventUpdating)expectedResult);
        var mapperMock = new Mock<IMapper>(MockBehavior.Strict);
        mapperMock.Setup(m => m.Map<CalendarEvent>(It.IsAny<UpdateEventCommand>()))
            .Returns((UpdateEventCommand src) =>
                new CalendarEvent(src.Id, src.UserId, src.Subject, src.Description, src.Begin, src.End));
        mapperMock.Setup(m => m.Map<UpdateEventCommandResult>(It.IsAny<ResultOfEventUpdating>()))
            .Returns((ResultOfEventUpdating src) => (UpdateEventCommandResult)src);

        var sut = new UpdateEventCommandHandler(calendarMock.Object, mapperMock.Object);





        var result = await sut.Handle(command, CancellationToken.None);





        result.Should().Be(expectedResult);

        calendarMock.Verify(c => c.UpdateAsync(It.Is<ICalendarEvent>(e =>
            e.UserId == command.UserId &&
            e.Id == command.Id &&
            e.Subject == command.Subject &&
            e.Description == command.Description &&
            e.Begin == command.Begin &&
            e.End == command.End)), Times.Once);

        mapperMock.Verify(m => m.Map<CalendarEvent>(It.Is<UpdateEventCommand>(c => c == command)), Times.Once);
        mapperMock.Verify(m => m.Map<UpdateEventCommandResult>(It.Is<ResultOfEventUpdating>(r => r == (ResultOfEventUpdating)expectedResult)), Times.Once);
    }
}