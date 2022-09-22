using AutoFixture;
using AutoFixture.AutoMoq;
using AutoMapper;
using Calendar.Application.Commands.CreateEvent;
using Calendar.Domain;
using Calendar.Domain.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace Calendar.Application.UnitTests.Commands.CreateEvent;

public class CreateEventCommandHandlerTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    public async Task Handle_CreateCommand_DifferentResult(int id)
    {
        var fixture = new Fixture().Customize(new AutoMoqCustomization { ConfigureMembers = true });
        var command = fixture.Build<CreateEventCommand>()
            .With(p => p.Begin, DateTime.Today)
            .With(p => p.End, DateTime.Today.AddHours(1))
            .Create();

        var calendarMock = new Mock<ICalendar>(MockBehavior.Strict);
        calendarMock.Setup(m => m.CreateAsync(It.IsAny<INewCalendarEvent>()))
            .ReturnsAsync(new ResultOfEventCreating(id == 0, id));
        var mapperMock = new Mock<IMapper>(MockBehavior.Strict);
        mapperMock.Setup(m => m.Map<NewCalendarEvent>(It.IsAny<CreateEventCommand>()))
            .Returns((CreateEventCommand src) =>
                new NewCalendarEvent(src.UserId, src.Subject, src.Description, src.Begin, src.End));
        mapperMock.Setup(m => m.Map<CreateEventCommandResult>(It.IsAny<ResultOfEventCreating>()))
            .Returns((ResultOfEventCreating src) => new CreateEventCommandResult(src.AlreadyExists, src.Id));

        var sut = new CreateEventCommandHandler(calendarMock.Object, mapperMock.Object);




        var result = await sut.Handle(command, CancellationToken.None);




        result.Id.Should().Be(id);
        result.AlreadyExists.Should().Be(id == 0);
    }
}