using AutoMapper;
using Calendar.Domain;
using Calendar.Domain.Abstract;

namespace Calendar.Infrastructure.Mapping;

/// <summary>
/// A profile for AutoMapper.
/// </summary>
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<INewCalendarEvent, EventEntity>()
            .Include<ICalendarEvent, EventEntity>();

        CreateMap<ICalendarEvent, EventEntity>();
        CreateMap<CalendarEvent, EventEntity>();
    }
}