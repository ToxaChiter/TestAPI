using Application.DTOs;
using AutoMapper;
using Core.Entities;

namespace Application.Mapping.Profiles;

public class EventProfile : Profile
{
    public EventProfile()
    {
        // Маппинг из Event в EventDTO
        CreateMap<Event, EventDTO>();

        // Маппинг из EventDTO в Event
        CreateMap<EventDTO, Event>()
            .ForMember(dest => dest.Id, opt => opt.Ignore());
    }
}
