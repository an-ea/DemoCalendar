using MediatR;

namespace Calendar.Application.Commands.UpdateEvent;

/// <summary>
/// Represents a command used for updating an event.
/// </summary>
public class UpdateEventCommand : EventCommand, IRequest<UpdateEventCommandResult>
{
    /// <summary>
    /// Id of existing event.
    /// </summary>
    public int Id { get; init; }
}