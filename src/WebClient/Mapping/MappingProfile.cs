using AutoMapper;
using WebClient.Models;
using WebClient.Services;

namespace WebClient.Mapping;

/// <summary>
/// A profile for AutoMapper.
/// </summary>
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<EventDto, Event>()
            .ForMember(d => d.Title, opt => opt.MapFrom(s => s.Subject))
            .ForMember(d => d.Start, opt => opt.MapFrom(s => s.Begin))
            .ForMember(d => d.EventId, opt => opt.MapFrom(s => s.Id));

        CreateMap<Event, EventDto>()
            .ForCtorParam(nameof(EventDto.Id), opt => opt.MapFrom(s => s.EventId))
            .ForCtorParam(nameof(EventDto.Subject), opt => opt.MapFrom(s => s.Title))
            .ForCtorParam(nameof(EventDto.Begin), opt => opt.MapFrom(s => s.Start));


        CreateMap<NewEvent, NewEventDto>()
            .ForCtorParam(nameof(NewEventDto.Subject), opt => opt.MapFrom(s => s.Title))
            .ForCtorParam(nameof(NewEventDto.Begin), opt => opt.MapFrom(s => s.Start));

    }
}