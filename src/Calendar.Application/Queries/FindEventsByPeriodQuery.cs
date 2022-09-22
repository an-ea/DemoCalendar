using Calendar.Application.Dto;
using MediatR;

namespace Calendar.Application.Queries;

/// <summary>
/// Represents a query used for finding events by period.
/// </summary>
public class FindEventsByPeriodQuery : Request, IRequest<IEnumerable<EventDto>>
{
    public DateTime Begin { get; init; }

    public DateTime End { get; init; }
}