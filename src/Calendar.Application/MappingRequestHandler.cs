using AutoMapper;
using Calendar.Domain.Abstract;

namespace Calendar.Application;

/// <summary>
/// Represents an abstract mapping request handler.
/// </summary>
public abstract class MappingRequestHandler : RequestHandler
{
    protected MappingRequestHandler(ICalendar calendar, IMapper mapper) : base(calendar)
    {
        Mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    /// <summary>
    /// An object mapper.
    /// </summary>
    protected IMapper Mapper { get; }
}