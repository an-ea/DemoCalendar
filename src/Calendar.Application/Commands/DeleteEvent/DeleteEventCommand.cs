using MediatR;

namespace Calendar.Application.Commands.DeleteEvent;

/// <summary>
/// Represents a command used for deleting an event.
/// </summary>
public class DeleteEventCommand : RequestById, IRequest<bool>
{
}