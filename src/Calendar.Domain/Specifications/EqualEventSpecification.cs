using System.Linq.Expressions;
using Calendar.Domain.Abstract;

namespace Calendar.Domain.Specifications;

/// <summary>
/// Represents a specification used for finding an equal event.
/// </summary>
internal class EqualEventSpecification : IEventEntitySpecification
{
    private readonly INewCalendarEvent _event;

    /// <summary>
    /// Initializes a new instance of the <see cref="EqualEventSpecification" /> class.
    /// </summary>
    /// <param name="event">An event used for equality check.</param>
    /// <exception cref="ArgumentNullException"></exception>
    public EqualEventSpecification(INewCalendarEvent @event)
    {
        _event = @event ?? throw new ArgumentNullException(nameof(@event));
    }

    public Expression<Func<EventEntity, bool>> IsSatisfiedBy => e =>
        e.UserId == _event.UserId &&
        e.Subject == _event.Subject &&
        e.Description == _event.Description &&
        e.Begin == _event.Begin &&
        e.End == _event.End;
}