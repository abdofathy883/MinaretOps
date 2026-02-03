using AutoMapper;
using Core.DTOs.EmployeeOnBoarding;
using Core.Models;

namespace Infrastructure.MappingProfiles
{
    public class EmpInvitationProfile : Profile
    {
        public EmpInvitationProfile()
        {
            CreateMap<EmployeeOnBoardingInvitation, InvitationDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.ExpiresAt, opt => opt.MapFrom(src => src.ExpiresAt))
                .ForMember(dest => dest.CompletedAt, opt => opt.MapFrom(src => src.CompletedAt))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.InvitedByUserName, opt => opt.MapFrom(src => 
                    src.InvitedBy != null ? $"{src.InvitedBy.FirstName} {src.InvitedBy.LastName}" : null))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName));
        }
    }
}
