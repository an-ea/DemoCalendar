using Calendar.Domain.Abstract;

namespace Calendar.Domain;

/// <summary>
/// Provides an implementation of <see cref="INewCalendarEvent" />.
/// </summary>
public class NewCalendarEvent : INewCalendarEvent
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NewCalendarEvent" /> class.
    /// </summary>
    /// <param name="userId">A user id.</param>
    /// <param name="subject">A subject of event.</param>
    /// <param name="description">A description of event.</param>
    /// <param name="begin">A begin of event.</param>
    /// <param name="end">An end of event.</param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public NewCalendarEvent(int userId, string subject, string description, DateTime begin, DateTime end)
    {
        UserId = userId > 0 ? userId : throw new ArgumentOutOfRangeException(nameof(userId), userId, null);
        Subject = subject ?? throw new ArgumentNullException(nameof(subject));
        Description = description ?? throw new ArgumentNullException(nameof(description));

        if (end <= begin)
            throw new ArgumentOutOfRangeException($"{nameof(end)} is less than or equal to {nameof(begin)}");

        Begin = begin;
        End = end;
    }

    public int UserId { get; }
    public string Subject { get; }
    public string Description { get; }
    public DateTime Begin { get; }
    public DateTime End { get; }
}