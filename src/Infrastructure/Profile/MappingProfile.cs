using AutoMapper;
using Core.Entities;
using Core.DTOs;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<FaultReport, NotificationReadDto>()
            // Enum olan Priority'yi string'e çeviriyoruz
            .ForMember(dest => dest.Priority, opt => opt.MapFrom(src => src.Priority.ToString()))
            // Enum olan Status'u string'e çeviriyoruz
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            // İlişkili tablodaki UserName'i çekiyoruz
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName));
            
        CreateMap<User, UserDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id)) // Açıkça belirtelim
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.ToString()));
    }
}