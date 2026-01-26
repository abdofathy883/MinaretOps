using AutoMapper;
using Core.DTOs.Announcements;
using Core.Models;

namespace Infrastructure.MappingProfiles
{
    public class AnnouncementProfile: Profile
    {
        public AnnouncementProfile()
        {
            CreateMap<Announcement, AnnouncementDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.Message, opt => opt.MapFrom(src => src.Message))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.AnnouncementLinks, opt => opt.MapFrom(src => src.AnnouncementLinks));

            CreateMap<AnnouncementLink, AnnouncementLinkDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Link, opt => opt.MapFrom(src => src.Link));
        }
    }
}
