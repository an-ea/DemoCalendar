using AutoMapper;
using Calendar.Application.Dto;
using Calendar.Domain.Abstract;
using MediatR;

namespace Calendar.Application.Queries;

/// <summary>
/// Represents a query handler used for finding an event by id.
/// </summary>
public class FindEventByIdQueryHandler : MappingRequestHandler, IRequestHandler<FindEventByIdQuery, EventDto?>
{
    public FindEventByIdQueryHandler(ICalendar calendar, IMapper mapper) : base(calendar, mapper)
    {
    }

    public async Task<EventDto?> Handle(FindEventByIdQuery request, CancellationToken cancellationToken)
    {
        var @event = await Calendar.FindAsync(request.UserId, request.Id);
        return @event == null ? null : Mapper.Map<EventDto>(@event);
    }
}