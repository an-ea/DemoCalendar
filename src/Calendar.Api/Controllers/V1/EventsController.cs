using AutoMapper;
using Calendar.Api.Models;
using Calendar.Api.Validation;
using Calendar.Application.Commands.CreateEvent;
using Calendar.Application.Commands.DeleteEvent;
using Calendar.Application.Commands.UpdateEvent;
using Calendar.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Calendar.Api.Controllers.V1;

[Route("api/v1/[controller]")]
[Authorize]
[ApiController]
public class EventsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    public EventsController(IMediator mediator, IMapper mapper)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }


    /// <summary>
    /// Gets events in a given range.
    /// </summary>
    /// <param name="dateTimeRange">A date time range.</param>
    /// <returns>A collection of events.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<EventModel>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IReadOnlyCollection<EventModel>>> GetEventsAsync([FromQuery]DateTimeRangeModel dateTimeRange)
    {
        var query = MapWithUserId<FindEventsByPeriodQuery>(dateTimeRange);
        var result = await _mediator.Send(query);

        var models = _mapper.Map<IReadOnlyCollection<EventModel>>(result);
        return Ok(models);
    }


    /// <summary>
    /// Gets an event by id.
    /// </summary>
    /// <param name="id">An id of event.</param>
    /// <returns>A found event.</returns>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(EventModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EventModel>> GetEventAsync([ValidId]int id)
    {
        var query = new FindEventByIdQuery { Id = id, UserId = GetUserId() };

        var result = await _mediator.Send(query);
        if (result == null)
            return NotFound();

        var model = _mapper.Map<EventModel>(result);
        return Ok(model);
    }

    /// <summary>
    /// Creates a new event.
    /// </summary>
    /// <param name="event">A new event.</param>
    /// <returns>A created event info.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(NewEventInfoModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<NewEventInfoModel>> CreateEventAsync([FromBody]NewEventModel @event)
    {
        var command = MapWithUserId<CreateEventCommand>(@event);
        var result = await _mediator.Send(command);

        if (result.Id == 0)
            return Conflict();
        return Ok(new NewEventInfoModel(result.Id));
    }


    /// <summary>
    /// Updates an existing event.
    /// </summary>
    /// <param name="event">An updated event.</param>
    /// <returns>A result of updating.</returns>
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult> UpdateEventAsync([FromBody]EventModel @event)
    {
        var command = MapWithUserId<UpdateEventCommand>(@event);
        var result = await _mediator.Send(command);

        return result switch
        {
            UpdateEventCommandResult.Success => NoContent(),
            UpdateEventCommandResult.NotFound => NotFound(),
            UpdateEventCommandResult.AlreadyExists => Conflict(),
            _ => throw new ArgumentOutOfRangeException(nameof(result), result, null)
        };
    }


    /// <summary>
    /// Deletes an event.
    /// </summary>
    /// <param name="id">An id of event to delete.</param>
    /// <returns>A result of deleting.</returns>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteEventAsync([ValidId]int id)
    {
        var command = new DeleteEventCommand { UserId = GetUserId(), Id = id };
        var result = await _mediator.Send(command);
        return result ? NoContent() : NotFound();
    }

    /// <summary>
    /// Sets a user id for AutoMapper.
    /// </summary>
    /// <param name="opt">AutoMapper options.</param>
    private void SetUserIdForMapping(IMappingOperationOptions opt) => opt.Items["userId"] = GetUserId();

    /// <summary>
    /// Gets a user id from current context.
    /// </summary>
    /// <returns>A user id.</returns>
    private int GetUserId() => int.Parse(User.FindFirst("sub")!.Value);

    /// <summary>
    /// Maps an object and adds user id.
    /// </summary>
    /// <typeparam name="TOut">A type of mapped object.</typeparam>
    /// <param name="obj">A source object.</param>
    /// <returns>A mapped object.</returns>
    private TOut MapWithUserId<TOut>(object obj) => _mapper.Map<TOut>(obj, SetUserIdForMapping);

}