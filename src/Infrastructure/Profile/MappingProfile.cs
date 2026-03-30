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
            
        // Eğer tam tersi de lazımsa: CreateMap<NotificationReadDto, FaultReport>();
    }
}