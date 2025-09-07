using AutoMapper;
using Core.DTOs.Notifications;
using Core.Models;

namespace Infrastructure.MappingProfiles
{
    public class NotificationProfile: Profile
    {
        public NotificationProfile()
        {
            CreateMap<PushNotification, NotificationDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.Body, opt => opt.MapFrom(src => src.Body))
                .ForMember(dest => dest.Url, opt => opt.MapFrom(src => src.Url))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.IsRead, opt => opt.MapFrom(src => src.IsRead));
        }
    }
}
