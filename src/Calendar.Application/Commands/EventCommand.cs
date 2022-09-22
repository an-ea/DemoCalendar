namespace Calendar.Application.Commands;

/// <summary>
/// Represents a command used for event actions.
/// </summary>
public abstract class EventCommand : Request
{
    public string Subject { get; init; } = null!;

    public string Description { get; init; } = null!;

    public DateTime Begin { get; init; }

    public DateTime End { get; init; }
}