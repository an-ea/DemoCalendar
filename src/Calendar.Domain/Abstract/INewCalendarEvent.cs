namespace Calendar.Domain.Abstract;

/// <summary>
/// Defines a new user event.
/// </summary>
public interface INewCalendarEvent
{
    /// <summary>
    /// An id of user.
    /// </summary>
    int UserId { get; }

    /// <summary>
    /// A subject of event.
    /// </summary>
    string Subject { get; }

    /// <summary>
    /// A description of event.
    /// </summary>
    string Description { get; }

    /// <summary>
    /// A begin of event.
    /// </summary>
    DateTime Begin { get; }

    /// <summary>
    /// An end of event.
    /// </summary>
    DateTime End { get; }
}