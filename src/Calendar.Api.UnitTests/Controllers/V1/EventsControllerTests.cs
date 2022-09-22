using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoMapper;
using Calendar.Api.Controllers.V1;
using Calendar.Api.Mapping;
using Calendar.Api.Models;
using Calendar.Application.Commands.CreateEvent;
using Calendar.Application.Commands.DeleteEvent;
using Calendar.Application.Commands.UpdateEvent;
using Calendar.Application.Dto;
using Calendar.Application.Queries;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Calendar.Api.UnitTests.Controllers.V1;

public class EventsControllerTests
{
    public const int UserId = 1;

    [Fact]
    public async Task GetEventsAsync_ValidPeriod_OkResult()
    {
        var fixture = CreateFixture();
        var events = Enumerable.Range(1, 3).Select(_ => fixture.Create<EventDto>()).ToArray();
        
        var mediatorMock = new Mock<IMediator>(MockBehavior.Strict);
        mediatorMock
            .Setup(m => m.Send(It.IsAny<FindEventsByPeriodQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(events);

        var begin = events[^1].Begin;
        var end   = events[^1].End;

        var sut = CreateController(mediatorMock.Object);





        var response = await sut.GetEventsAsync(new DateTimeRangeModel(begin, end));





        var result = response.Result.Should().BeOfType<OkObjectResult>();
        result.Which.StatusCode.Should().Be(StatusCodes.Status200OK);
        var value = result.Which.Value.Should().BeAssignableTo<IEnumerable<EventModel>>();
        
        foreach (var (res, exp) in value.Subject.Zip(events))
            res.Should().BeEquivalentTo(exp);

        mediatorMock.Verify(m => m.Send(
                It.Is<FindEventsByPeriodQuery>(q => q.Begin == begin && q.End == end && q.UserId == UserId),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    
    [Fact]
    public async Task GetEventAsync_ValidId_OkResult()
    {
        var expectedEvent = CreateFixture().Create<EventDto>();

        var mediatorMock = new Mock<IMediator>(MockBehavior.Strict);
        mediatorMock
            .Setup(m => m.Send(It.IsAny<FindEventByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedEvent);

        var sut = CreateController(mediatorMock.Object);




        var response = await sut.GetEventAsync(expectedEvent.Id);




        var result = response.Result.Should().BeOfType<OkObjectResult>();
        result.Which.StatusCode.Should().Be(StatusCodes.Status200OK);
        result.Which.Value.Should().BeOfType<EventModel>()
            .Subject.Should().BeEquivalentTo(expectedEvent);

        mediatorMock.Verify(m => m.Send(
                It.Is<FindEventByIdQuery>(q => q.Id == expectedEvent.Id && q.UserId == UserId),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    
    [Theory]
    [InlineData(3, StatusCodes.Status200OK)]
    [InlineData(0, StatusCodes.Status409Conflict)]
    public async Task CreateEventAsync_DifferentEvents_DifferentResults(int id, int expectedCode)
    {
        var newEvent = CreateFixture().Build<NewEventModel>()
            .With(p => p.Begin, DateTime.Today)
            .With(p => p.End,   DateTime.Today.AddHours(1))
            .Create();

        var mediatorMock = new Mock<IMediator>(MockBehavior.Strict);
        mediatorMock
            .Setup(m => m.Send(It.IsAny<CreateEventCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CreateEventCommandResult(id != 0, id));
        
        var sut = CreateController(mediatorMock.Object);





        var response = await sut.CreateEventAsync(newEvent);




        if (expectedCode == StatusCodes.Status409Conflict)
        {
            var conflictResult = response.Result.Should().BeOfType<ConflictResult>();
            conflictResult.Which.StatusCode.Should().Be(expectedCode);
            return;
        }
        
        var okResult = response.Result.Should().BeOfType<OkObjectResult>();
        okResult.Which.StatusCode.Should().Be(expectedCode);
        okResult.Which.Value.Should().BeOfType<NewEventInfoModel>().Which.Id.Should().Be(id);

        mediatorMock.Verify(m => m.Send(
                It.Is<CreateEventCommand>(c =>
                    c.Subject == newEvent.Subject &&
                    c.Description == newEvent.Description &&
                    c.Begin == newEvent.Begin &&
                    c.End == newEvent.End &&
                    c.UserId == UserId),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }


    [Theory]
    [InlineData(UpdateEventCommandResult.Success, StatusCodes.Status204NoContent)]
    [InlineData(UpdateEventCommandResult.NotFound, StatusCodes.Status404NotFound)]
    [InlineData(UpdateEventCommandResult.AlreadyExists, StatusCodes.Status409Conflict)]
    public async Task UpdateEventAsync_DifferentData_DifferentResults(UpdateEventCommandResult expectedResult, int expectedCode)
    {
        var fixture = CreateFixture();

        var @event = fixture.Build<EventModel>()
            .With(p => p.Begin, DateTime.Today)
            .With(p => p.End,   DateTime.Today.AddHours(1))
            .Create();

        var mediatorMock = new Mock<IMediator>(MockBehavior.Strict);
        mediatorMock
            .Setup(m => m.Send(It.IsAny<UpdateEventCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var sut = CreateController(mediatorMock.Object);




        var response = await sut.UpdateEventAsync(@event);




        var result = response.Should().BeAssignableTo<StatusCodeResult>();
        result.Which.StatusCode.Should().Be(expectedCode);
        mediatorMock.Verify(m => m.Send(
            It.Is<UpdateEventCommand>(c => 
                c.Id == @event.Id &&
                c.Subject == @event.Subject &&
                c.Description == @event.Description &&
                c.Begin == @event.Begin &&
                c.End == @event.End),
            It.IsAny<CancellationToken>()));
    }


    [Theory]
    [InlineData(true, StatusCodes.Status204NoContent)]
    [InlineData(false, StatusCodes.Status404NotFound)]
    public async Task Delete_DifferentData_DifferentResult(bool resultOfDeleting, int expectedCode)
    {
        var mediatorMock = new Mock<IMediator>(MockBehavior.Strict);
        mediatorMock
            .Setup(m => m.Send(It.IsAny<DeleteEventCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(resultOfDeleting);
        var eventId = DateTime.Now.Millisecond + 1; 
        var sut = CreateController(mediatorMock.Object);




        var response = await sut.DeleteEventAsync(eventId);




        var result = response.Should().BeAssignableTo<StatusCodeResult>();
        result.Which.StatusCode.Should().Be(expectedCode);
        mediatorMock.Verify(m => m.Send(It.Is<DeleteEventCommand>(c => c.Id == eventId), It.IsAny<CancellationToken>()));
    }


    private static IFixture CreateFixture() => new Fixture().Customize(new AutoMoqCustomization { ConfigureMembers = true });

    private static EventsController CreateController(IMediator mediator)
    {
        var controller = new EventsController(
            mediator,
            new Mapper(new MapperConfiguration(cfg => cfg.AddProfile(new MappingProfile()))));

        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new("sub", UserId.ToString()) }, "mock"));

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };

        return controller;
    }
}