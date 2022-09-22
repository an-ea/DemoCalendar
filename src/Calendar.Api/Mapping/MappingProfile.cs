using AutoMapper;
using Calendar.Api.Models;
using Calendar.Application.Commands.CreateEvent;
using Calendar.Application.Commands.UpdateEvent;
using Calendar.Application.Dto;
using Calendar.Application.Queries;

namespace Calendar.Api.Mapping;

/// <summary>
/// A profile for AutoMapper.
/// </summary>
internal class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<DateTimeRangeModel, FindEventsByPeriodQuery>().ForMember(d => d.UserId, MapUserId);

        CreateMap<NewEventModel, CreateEventCommand>().ForMember(d => d.UserId, MapUserId);

        CreateMap<EventModel, UpdateEventCommand>().ForMember(d => d.UserId, MapUserId);
        
        CreateMap<EventDto, EventModel>();
    }

    private static void MapUserId<TSource, TDestination>(IMemberConfigurationExpression<TSource, TDestination, int> config) =>
        config.MapFrom((_, _, _, ctx) => ctx.Options.Items["userId"]);
}