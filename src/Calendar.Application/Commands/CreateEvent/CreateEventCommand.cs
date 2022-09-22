using MediatR;

namespace Calendar.Application.Commands.CreateEvent;

/// <summary>
/// Represents a command used for creating a new event.
/// </summary>
public class CreateEventCommand : EventCommand, IRequest<CreateEventCommandResult>
{
}