using System.Net;
using Refit;
using WebClient.Exceptions;

namespace WebClient.Services;

/// <summary>
/// Provides an implementation of <see cref="ICalendarService" /> uses a given <see cref="ICalendarApi" />.
/// </summary>
public class CalendarService : ICalendarService
{
    private readonly ICalendarApi _calendarApi;

    /// <summary>
    /// Initializes a new instance of the <see cref="CalendarService" /> class.
    /// </summary>
    /// <param name="calendarApi">A calendar API.</param>
    /// <exception cref="ArgumentNullException"></exception>
    public CalendarService(ICalendarApi calendarApi)
    {
        _calendarApi = calendarApi ?? throw new ArgumentNullException(nameof(calendarApi));
    }

    public async Task<IEnumerable<EventDto>> FindEventsAsync(DateOnly begin, DateOnly end)
    {
        if (end < begin)
            throw new ArgumentOutOfRangeException($"{nameof(end)} is less than {nameof(begin)}");

        return await _calendarApi.GetEventsAsync(begin, end);
    }

    public async Task<NewEventInfoDto> CreateEventAsync(NewEventDto newEvent)
    {
        ArgumentNullException.ThrowIfNull(newEvent);
        
        try
        {
            return await _calendarApi.CreateEventAsync(newEvent);
        }
        catch (ApiException e)
        {
            ThrowEventExistsExceptionIfConflict(e.StatusCode);
            throw;
        }
    }

    public async Task UpdateEventAsync(EventDto updatedEvent)
    {
        ArgumentNullException.ThrowIfNull(updatedEvent);
        
        try
        {
            await _calendarApi.UpdateEventAsync(updatedEvent);
        }
        catch (ApiException e)
        {
            ThrowEventExistsExceptionIfConflict(e.StatusCode);
            throw;
        }
    }

    public async Task DeleteEventAsync(int id)
    {
        if (id < 1)
            throw new ArgumentOutOfRangeException(nameof(id), id, $"{nameof(id)} is less than 1.");

        await _calendarApi.DeleteEventAsync(id);
    }

    private static void ThrowEventExistsExceptionIfConflict(HttpStatusCode code)
    {
        if (code == HttpStatusCode.Conflict)
            throw new EventExistsException("The event already exists.");
    }

}