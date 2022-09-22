namespace Calendar.Domain.Abstract;

/// <summary>
/// Defines a calendar used for manipulating user events.
/// </summary>
public interface ICalendar
{
    /// <summary>
    /// Finds events in a given <see cref="DateTimeRange" />.
    /// </summary>
    /// <param name="userId">A user id.</param>
    /// <param name="range">A datetime range.</param>
    /// <returns>An <see cref="IEnumerable{ICalendarEvent}" /> that contains user events.</returns>
    Task<IEnumerable<ICalendarEvent>> FindAsync(int userId, DateTimeRange range);


    /// <summary>
    /// Finds an event by user id and event id.
    /// </summary>
    /// <param name="userId">A user id.</param>
    /// <param name="eventId">An event id.</param>
    /// <returns>A <see cref="ICalendarEvent" />, or <see langword="null" /> if no such event is found.</returns>
    Task<ICalendarEvent?> FindAsync(int userId, int eventId);


    /// <summary>
    /// Finds events satisfying a given specification.
    /// </summary>
    /// <param name="specification">A specification used for filtering events.</param>
    /// <returns><see cref="IEnumerable{ICalendarEvent}" /> that contains user events.</returns>
    Task<IEnumerable<ICalendarEvent>> FindAsync(IEventEntitySpecification specification);


    /// <summary>
    /// Determines whether any event satisfies a specification.
    /// </summary>
    /// <param name="specification">A specification used for filtering events.</param>
    /// <returns>
    /// <see langword="true" /> - if any events satisfy a given specification; otherwise, <see langword="false" />.
    /// </returns>
    Task<bool> AnyAsync(IEventEntitySpecification specification);


    /// <summary>
    /// Creates a new user event.
    /// </summary>
    /// <param name="event">A new user event.</param>
    /// <returns>A result of event creating.</returns>
    Task<ResultOfEventCreating> CreateAsync(INewCalendarEvent @event);


    /// <summary>
    /// Updates an event.
    /// </summary>
    /// <param name="event">An updated event that will replace the exist one.</param>
    /// <returns>A result of event updating.</returns>
    Task<ResultOfEventUpdating> UpdateAsync(ICalendarEvent @event);


    /// <summary>
    /// Deletes an event.
    /// </summary>
    /// <param name="userId">A user id.</param>
    /// <param name="eventId">An event id.</param>
    /// <returns><see langword="true"/> - an event has been deleted, <see langword="false"/> - an event wasn't found.</returns>
    Task<bool> DeleteAsync(int userId, int eventId);
}