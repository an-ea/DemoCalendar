using Refit;

namespace WebClient.Services;

/// <summary>
/// Defines an API of the Calendar service.
/// </summary>
public interface ICalendarApi
{
    [Get("")]
    Task<IEnumerable<EventDto>> GetEventsAsync(
        [Query(Format = CalendarApiConstants.DateFormat)] DateOnly begin,
        [Query(Format = CalendarApiConstants.DateFormat)] DateOnly end);

    [Post("")]
    Task<NewEventInfoDto> CreateEventAsync(NewEventDto newEvent);

    [Put("")]
    Task UpdateEventAsync(EventDto updatedEvent);

    [Delete("/{id}")]
    Task DeleteEventAsync(int id);

}

internal static class CalendarApiConstants
{
    public const string DateFormat = "yyyy-MM-dd";
}