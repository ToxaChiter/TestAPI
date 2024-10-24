using Application.DTOs;
using AutoMapper;
using Core.Entities;

namespace Application.Mapping.Profiles;

public class ParticipantProfile : Profile
{
    public ParticipantProfile()
    {
        // Маппинг из Participant в ParticipantDTO
        CreateMap<Participant, ParticipantDTO>();

        // Маппинг из ParticipantDTO в Participant
        CreateMap<ParticipantDTO, Participant>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore());
    }
}
