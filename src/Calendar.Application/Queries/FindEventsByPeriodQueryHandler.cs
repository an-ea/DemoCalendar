using AutoMapper;
using Calendar.Application.Dto;
using Calendar.Domain;
using Calendar.Domain.Abstract;
using MediatR;

namespace Calendar.Application.Queries;

/// <summary>
/// Represents a query handler used for finding events by period.
/// </summary>
public class FindEventsByPeriodQueryHandler : MappingRequestHandler, IRequestHandler<FindEventsByPeriodQuery, IEnumerable<EventDto>>
{
    public FindEventsByPeriodQueryHandler(ICalendar calendar, IMapper mapper) : base(calendar, mapper)
    {
    }

    public async Task<IEnumerable<EventDto>> Handle(FindEventsByPeriodQuery request, CancellationToken cancellationToken)
    {
        var events = await Calendar.FindAsync(request.UserId, new DateTimeRange(request.Begin, request.End));
        return Mapper.Map<IReadOnlyCollection<EventDto>>(events);
    }
}