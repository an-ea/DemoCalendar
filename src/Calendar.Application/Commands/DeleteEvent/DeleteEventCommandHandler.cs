using Calendar.Domain.Abstract;
using MediatR;

namespace Calendar.Application.Commands.DeleteEvent;

/// <summary>
/// Represents a command handler used for creating a new event.
/// </summary>
public class DeleteEventCommandHandler : RequestHandler, IRequestHandler<DeleteEventCommand, bool>
{
    public DeleteEventCommandHandler(ICalendar calendar) : base(calendar)
    {
    }

    public async Task<bool> Handle(DeleteEventCommand command, CancellationToken cancellationToken) =>
        await Calendar.DeleteAsync(command.UserId, command.Id).ConfigureAwait(false);
}