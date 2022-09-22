using Calendar.Application.Commands.DeleteEvent;
using Calendar.Domain.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace Calendar.Application.UnitTests.Commands.DeleteEvent;

public class DeleteEventCommandHandlerTests
{
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Handle_DeleteCommand_DifferentResults(bool expectedResult)
    {
        var calendarMock = new Mock<ICalendar>();
        calendarMock.Setup(m => m.DeleteAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(expectedResult);
        var command = new DeleteEventCommand { Id = DateTime.Now.Millisecond + 1, UserId = 1 };
        var sut = new DeleteEventCommandHandler(calendarMock.Object);



        var result = await sut.Handle(command, CancellationToken.None);



        result.Should().Be(expectedResult);
        calendarMock.Verify(c => c.DeleteAsync(
                It.Is<int>(id => id == command.UserId),
                It.Is<int>(id => id == command.Id)),
            Times.Once);
    }

}