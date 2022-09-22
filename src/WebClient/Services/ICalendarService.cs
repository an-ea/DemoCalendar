namespace WebClient.Services;

/// <summary>
/// Defines a calendar used for manipulating user events.
/// </summary>
public interface ICalendarService
{
    /// <summary>
    /// Finds events in a given period.
    /// </summary>
    /// <param name="begin">A begin of period.</param>
    /// <param name="end">An end of period.</param>
    /// <returns>An <see cref="IEnumerable{EventDto}"></see> that contains events.</returns>
    Task<IEnumerable<EventDto>> FindEventsAsync(DateOnly begin, DateOnly end);

    /// <summary>
    /// Creates a new event.
    /// </summary>
    /// <param name="newEvent">A new event.</param>
    /// <returns>Info about created event.</returns>
    Task<NewEventInfoDto> CreateEventAsync(NewEventDto newEvent);

    /// <summary>
    /// Updates an existing event.
    /// </summary>
    /// <param name="updatedEvent">An updated event.</param>
    /// <returns>A task that represents the asynchronous update operation.</returns>
    Task UpdateEventAsync(EventDto updatedEvent);

    /// <summary>
    /// Deletes an event.
    /// </summary>
    /// <param name="id">An id of event to delete.</param>
    /// <returns>A task that represents the asynchronous delete operation.</returns>
    Task DeleteEventAsync(int id);
}