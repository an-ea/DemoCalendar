using AutoMapper;
using Calendar.Domain;
using Calendar.Domain.Abstract;
using MediatR;

namespace Calendar.Application.Commands.CreateEvent;

/// <summary>
/// Represents a command handler used for creating a new event.
/// </summary>
public class CreateEventCommandHandler : MappingRequestHandler, IRequestHandler<CreateEventCommand, CreateEventCommandResult>
{
    public CreateEventCommandHandler(ICalendar calendar, IMapper mapper) : base(calendar, mapper)
    {
    }

    public async Task<CreateEventCommandResult> Handle(CreateEventCommand request, CancellationToken cancellationToken)
    {
        var @event = Mapper.Map<NewCalendarEvent>(request);
        var result = await Calendar.CreateAsync(@event).ConfigureAwait(false);
        return Mapper.Map<CreateEventCommandResult>(result);
    }
}