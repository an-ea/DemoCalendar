using Calendar.Domain.Abstract;

namespace Calendar.Domain;

public class EventEntity : ICalendarEvent
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string Subject { get; set; } = null!;

    public string Description { get; set; } = null!;

    public DateTime Begin { get; set; }

    public DateTime End { get; set; }
}