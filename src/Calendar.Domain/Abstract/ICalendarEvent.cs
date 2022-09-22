namespace Calendar.Domain.Abstract;

/// <summary>
/// Defines an event.
/// </summary>
public interface ICalendarEvent : INewCalendarEvent
{
    /// <summary>
    /// An id of event.
    /// </summary>
    public int Id { get; }
}