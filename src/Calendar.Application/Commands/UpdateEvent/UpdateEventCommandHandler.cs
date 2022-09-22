using AutoMapper;
using Calendar.Domain;
using Calendar.Domain.Abstract;
using MediatR;

namespace Calendar.Application.Commands.UpdateEvent;

/// <summary>
/// Represents a command handler used for updating an event.
/// </summary>
public class UpdateEventCommandHandler : MappingRequestHandler, IRequestHandler<UpdateEventCommand, UpdateEventCommandResult>
{
    public UpdateEventCommandHandler(ICalendar calendar, IMapper mapper) : base(calendar, mapper)
    {
    }

    public async Task<UpdateEventCommandResult> Handle(UpdateEventCommand request, CancellationToken cancellationToken)
    {
        var @event = Mapper.Map<CalendarEvent>(request);
        var result = await Calendar.UpdateAsync(@event).ConfigureAwait(false);
        return Mapper.Map<UpdateEventCommandResult>(result);
    }
}