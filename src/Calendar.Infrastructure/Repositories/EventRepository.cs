using System.Runtime.CompilerServices;
using AutoMapper;
using Calendar.Domain;
using Calendar.Domain.Abstract;
using Microsoft.EntityFrameworkCore;

namespace Calendar.Infrastructure.Repositories;

/// <summary>
/// Provides an implementation of <see cref="IEventRepository" /> uses a given <see cref="CalendarDbContext" />.
/// </summary>
public class EventRepository : IEventRepository, IDisposable, IAsyncDisposable
{
    private readonly CalendarDbContext _context;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="EventRepository" /> class.
    /// </summary>
    /// <param name="context">A database context.</param>
    /// <param name="mapper">A mapper.</param>
    /// <exception cref="ArgumentNullException"></exception>
    public EventRepository(CalendarDbContext context, IMapper mapper)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    private IQueryable<EventEntity> AsNoTracking() => _context.Events.AsNoTracking();

    public async Task<IEnumerable<ICalendarEvent>> FindAsync(int userId, DateTimeRange range)
    {
        EnsureValidId(userId);

        return await AsNoTracking()
            .Where(e => e.UserId == userId && e.Begin >= range.Begin && e.Begin < range.End)
            .ToArrayAsync()
            .ConfigureAwait(false);
    }

    public async Task<ICalendarEvent?> FindAsync(int userId, int eventId)
    {
        EnsureValidId(userId);
        EnsureValidId(eventId);

        return await AsNoTracking()
            .SingleOrDefaultAsync(e => e.Id == eventId && e.UserId == userId)
            .ConfigureAwait(false);
    }

    public async Task<IEnumerable<ICalendarEvent>> FindAsync(IEventEntitySpecification specification)
    {
        ArgumentNullException.ThrowIfNull(specification);

        return await AsNoTracking()
            .Where(specification.IsSatisfiedBy)
            .ToArrayAsync()
            .ConfigureAwait(false);
    }

    public async Task<bool> AnyAsync(IEventEntitySpecification specification)
    {
        ArgumentNullException.ThrowIfNull(specification);

        return await AsNoTracking()
            .AnyAsync(specification.IsSatisfiedBy)
            .ConfigureAwait(false);
    }

    public async Task<int> CreateAsync(INewCalendarEvent @event)
    {
        ArgumentNullException.ThrowIfNull(@event);

        var entity = ToEntity(@event);
        await _context.Events.AddAsync(entity).ConfigureAwait(false);
        await _context.SaveChangesAsync().ConfigureAwait(false);
        return entity.Id;
    }

    public async Task<bool> UpdateAsync(ICalendarEvent @event)
    {
        ArgumentNullException.ThrowIfNull(@event);

        var entity = await FindEvent(@event.Id).ConfigureAwait(false);
        if (entity == null)
            return false;

        var updatedEntity = ToEntity(@event);
        _context.Entry(entity).CurrentValues.SetValues(updatedEntity);
        return await SaveAsync().ConfigureAwait(false);
    }

    public async Task<bool> DeleteAsync(int userId, int eventId)
    {
        EnsureValidId(userId);
        EnsureValidId(eventId);

        var entity = await FindEvent(eventId).ConfigureAwait(false);
        if (entity == null)
            return false;

        _context.Events.Attach(entity);
        _context.Events.Remove(entity);
        return await SaveAsync().ConfigureAwait(false);
    }

    
    public void Dispose() => _context.Dispose();

    public ValueTask DisposeAsync() => _context.DisposeAsync();


    private ValueTask<EventEntity?> FindEvent(int id) => _context.Events.FindAsync(id);

    private EventEntity ToEntity(object obj) => _mapper.Map<EventEntity>(obj);

    private async Task<bool> SaveAsync()
    {
        var result = await _context.SaveChangesAsync().ConfigureAwait(false);
        return result != 0;
    }

    private static void EnsureValidId(int id, [CallerArgumentExpression("id")] string? idName = null)
    {
        if (id < 1)
            throw new ArgumentOutOfRangeException(idName, id, $"{idName} is less than 1.");
    }
   
}