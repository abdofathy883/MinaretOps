using AutoMapper;
using Core.DTOs.Contract;
using Core.Models;

namespace Infrastructure.MappingProfiles
{
    public class ContractProfile : Profile
    {
        public ContractProfile()
        {
            CreateMap<CustomContract, ContractDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.ClientId, opt => opt.MapFrom(src => src.Client.Id))
                .ForMember(dest => dest.ClientName, opt => opt.MapFrom(src => src.Client.Name))
                .ForMember(dest => dest.CurrencyId, opt => opt.MapFrom(src => src.Currency.Id))
                .ForMember(dest => dest.CurrencyName, opt => opt.MapFrom(src => src.Currency.Name))
                .ForMember(dest => dest.ServiceCost, opt => opt.MapFrom(src =>
                    src.Client.ClientServices != null && src.Client.ClientServices.Any()
                        ? src.Client.ClientServices.Select(cs => cs.ServiceCost).FirstOrDefault() ?? 0m
                        : 0m))
                .ForMember(dest => dest.ServiceName, opt => opt.MapFrom(src => 
                    src.Client.ClientServices != null 
                        ? src.Client.ClientServices.Select(cs => cs.Service.Title).FirstOrDefault() ?? string.Empty
                        : string.Empty))
                .ForMember(dest => dest.Country, opt => opt.MapFrom(src => src.Client.Country ?? string.Empty))
                .ForMember(dest => dest.AccountManagerName, opt => opt.MapFrom(src => 
                    src.Client.AccountManager != null 
                        ? $"{src.Client.AccountManager.FirstName} {src.Client.AccountManager.LastName}"
                        : string.Empty))
                .ForMember(dest => dest.BusinessType, opt => opt.MapFrom(src => src.Client.BusinessType))
                .ForMember(dest => dest.ContractDuration, opt => opt.MapFrom(src => src.ContractDuration))
                .ForMember(dest => dest.ContractTotal, opt => opt.MapFrom(src => src.ContractTotal))
                .ForMember(dest => dest.DueAmount, opt => opt.MapFrom(src => src.DueAmount))
                .ForMember(dest => dest.PaidAmount, opt => opt.MapFrom(src => src.PaidAmount));
        }
    }
}
