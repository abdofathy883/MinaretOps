using AutoMapper;
using Core.DTOs.Currency;
using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.MappingProfiles
{
    public class CurrencyProfile: Profile
    {
        public CurrencyProfile()
        {
            CreateMap<Currency, CurrencyDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.Code))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.DecimalPlaces, opt => opt.MapFrom(src => src.DecimalPlaces));

            CreateMap<ExchangeRate, ExchangeRateDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.FromCurrencyId, opt => opt.MapFrom(src => src.FromCurrencyId))
                .ForMember(dest => dest.ToCurrencyId, opt => opt.MapFrom(src => src.ToCurrencyId))
                .ForMember(dest => dest.Rate, opt => opt.MapFrom(src => src.Rate))
                .ForMember(dest => dest.EffectiveFrom, opt => opt.MapFrom(src => src.EffectiveFrom))
                .ForMember(dest => dest.EffectiveTo, opt => opt.MapFrom(src => src.EffectiveTo))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive));
        }
    }
}
