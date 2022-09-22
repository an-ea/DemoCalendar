using Calendar.Domain.Abstract;

namespace Calendar.Application;

/// <summary>
/// Represents an abstract request handler.
/// </summary>
public abstract class RequestHandler
{
    protected RequestHandler(ICalendar calendar)
    {
        Calendar = calendar ?? throw new ArgumentNullException(nameof(calendar));
    }

    /// <summary>
    /// A calendar for manipulating events.
    /// </summary>
    protected ICalendar Calendar { get; }
}