using AutoMapper;
using Calendar.Application.Commands.CreateEvent;
using Calendar.Application.Commands.UpdateEvent;
using Calendar.Application.Dto;
using Calendar.Domain;
using Calendar.Domain.Abstract;

namespace Calendar.Application.Mapping;

/// <summary>
/// A profile for AutoMapper.
/// </summary>
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<CreateEventCommand, NewCalendarEvent>();
        CreateMap<UpdateEventCommand, CalendarEvent>();
        CreateMap<ICalendarEvent, EventDto>();
        CreateMap<ResultOfEventUpdating, UpdateEventCommandResult>();
        CreateMap<ResultOfEventCreating, CreateEventCommandResult>();
    }
}