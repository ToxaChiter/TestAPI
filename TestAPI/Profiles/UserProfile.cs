using AutoMapper;
using TestAPI.DTOs;
using TestAPI.Models;

namespace TestAPI.Profiles;

public class UserProfile : Profile
{
    public UserProfile()
    {
        // Маппинг из User в UserDTO
        CreateMap<User, UserDTO>();

        // Маппинг из UserDTO в User
        CreateMap<UserDTO, User>()
            .ForMember(dest => dest.Id, opt => opt.Ignore());
    }
}
