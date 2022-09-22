using Calendar.Application.Dto;
using MediatR;

namespace Calendar.Application.Queries;

/// <summary>
/// Represents a query used for finding an event by id.
/// </summary>
public class FindEventByIdQuery : RequestById, IRequest<EventDto?>
{
}