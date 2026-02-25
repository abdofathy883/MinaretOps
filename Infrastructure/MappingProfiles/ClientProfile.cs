using AutoMapper;
using Core.DTOs.Clients;
using Core.Models;

namespace Infrastructure.MappingProfiles
{
    public class ClientProfile: Profile
    {
        public ClientProfile()
        {
            CreateMap<Client, ClientDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.CompanyName))
                .ForMember(dest => dest.PersonalPhoneNumber, opt => opt.MapFrom(src => src.PersonalPhoneNumber))
                .ForMember(dest => dest.CompanyNumber, opt => opt.MapFrom(src => src.CompanyNumber))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.BusinessDescription, opt => opt.MapFrom(src => src.BusinessDescription))
                .ForMember(dest => dest.DriveLink, opt => opt.MapFrom(src => src.DriveLink))
                .ForMember(dest => dest.BusinessType, opt => opt.MapFrom(src => src.BusinessType))
                .ForMember(dest => dest.BusinessActivity, opt => opt.MapFrom(src => src.BusinessActivity))
                .ForMember(dest => dest.CommercialRegisterNumber, opt => opt.MapFrom(src => src.CommercialRegisterNumber))
                .ForMember(dest => dest.TaxCardNumber, opt => opt.MapFrom(src => src.TaxCardNumber))
                .ForMember(dest => dest.Country, opt => opt.MapFrom(src => src.Country))
                .ForMember(dest => dest.AccountManagerId, opt => opt.MapFrom(src => src.AccountManagerId))
                .ForMember(dest => dest.AccountManagerName, opt => opt.MapFrom(src => src.AccountManager != null 
                ? $"{src.AccountManager.FirstName} {src.AccountManager.LastName}" : null))
                .ForMember(dest => dest.DiscordChannelId, opt => opt.MapFrom(src => src.DiscordChannelId))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.StatusNotes, opt => opt.MapFrom(src => src.StatusNotes))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt))
                .ForMember(dest => dest.ClientServices, opt => opt.MapFrom(src => src.ClientServices));
        }
    }
}
