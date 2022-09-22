using System.Runtime.CompilerServices;
using Calendar.Domain.Abstract;
using Calendar.Domain.Specifications;

namespace Calendar.Domain;

/// <summary>
/// Provides an implementation of <see cref="ICalendar" /> uses a given <see cref="IEventRepository" />.
/// </summary>
public class Calendar : ICalendar
{
    private readonly IEventRepository _repository;

    /// <summary>
    /// Initializes a new instance of the <see cref="Calendar" /> class.
    /// </summary>
    /// <param name="repository">An event repository.</param>
    /// <exception cref="ArgumentNullException"></exception>
    public Calendar(IEventRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<IEnumerable<ICalendarEvent>> FindAsync(int userId, DateTimeRange range)
    {
        EnsureValidId(userId);
        return await _repository.FindAsync(userId, range).ConfigureAwait(false);
    }

    public async Task<ICalendarEvent?> FindAsync(int userId, int eventId)
    {
        EnsureValidId(userId);
        EnsureValidId(eventId);
        return await _repository.FindAsync(userId, eventId).ConfigureAwait(false);
    }

    public async Task<IEnumerable<ICalendarEvent>> FindAsync(IEventEntitySpecification specification)
    {
        ArgumentNullException.ThrowIfNull(specification);
        return await _repository.FindAsync(specification).ConfigureAwait(false);
    }

    public async Task<bool> AnyAsync(IEventEntitySpecification specification)
    {
        ArgumentNullException.ThrowIfNull(specification);
        return await _repository.AnyAsync(specification).ConfigureAwait(false);
    }


    public async Task<ResultOfEventCreating> CreateAsync(INewCalendarEvent @event)
    {
        ArgumentNullException.ThrowIfNull(@event);

        if (await IsThereAnEqualEventAsync(@event).ConfigureAwait(false))
            return new ResultOfEventCreating(true, 0);

        var id = await _repository.CreateAsync(@event).ConfigureAwait(false);
        return new ResultOfEventCreating(false, id);
    }

    public async Task<ResultOfEventUpdating> UpdateAsync(ICalendarEvent @event)
    {
        ArgumentNullException.ThrowIfNull(@event);

        if (await IsThereAnEqualEventAsync(@event).ConfigureAwait(false))
            return ResultOfEventUpdating.AlreadyExists;

        var result = await _repository.UpdateAsync(@event).ConfigureAwait(false);
        return result ? ResultOfEventUpdating.Success : ResultOfEventUpdating.NotFound;
    }


    public async Task<bool> DeleteAsync(int userId, int eventId)
    {
        EnsureValidId(userId);
        EnsureValidId(eventId);
        return await _repository.DeleteAsync(userId, eventId).ConfigureAwait(false);
    }


    private static void EnsureValidId(int id, [CallerArgumentExpression("id")] string? idName = null)
    {
        if (id < 1)
            throw new ArgumentOutOfRangeException(idName, id, $"{idName} is less than 1.");
    }


    private async Task<bool> IsThereAnEqualEventAsync(INewCalendarEvent @event) =>
        await _repository.AnyAsync(new EqualEventSpecification(@event)).ConfigureAwait(false);

}