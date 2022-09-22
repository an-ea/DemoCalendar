using Calendar.Domain.Abstract;

namespace Calendar.Domain;

/// <summary>
/// Provides an implementation of <see cref="CalendarEvent" />.
/// </summary>
public class CalendarEvent : NewCalendarEvent, ICalendarEvent
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CalendarEvent" /> class.
    /// </summary>
    /// <param name="id">An id of event.</param>
    /// <param name="userId">A user id.</param>
    /// <param name="subject">A subject of event.</param>
    /// <param name="description">A description of event.</param>
    /// <param name="begin">A begin of event.</param>
    /// <param name="end">An end of event.</param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public CalendarEvent(int id, int userId, string subject, string description, DateTime begin, DateTime end) : 
        base(userId, subject, description, begin, end)
    {
        Id = id > 0 ? id : throw new ArgumentOutOfRangeException(nameof(id), id, null);
    }

    public int Id { get; }
}