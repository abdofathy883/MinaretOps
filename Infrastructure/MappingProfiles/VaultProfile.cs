using AutoMapper;
using Core.DTOs.Vault;
using Core.Enums;
using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.MappingProfiles
{
    public class VaultProfile : Profile
    {
        public VaultProfile()
        {
            CreateMap<Vault, VaultDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.VaultType, opt => opt.MapFrom(src => src.VaultType))
                .ForMember(dest => dest.BranchId, opt => opt.MapFrom(src => src.BranchId))
                .ForMember(dest => dest.BranchName, opt => opt.MapFrom(src => src.Branch.Name ?? null))
                .ForMember(dest => dest.CurrencyId, opt => opt.MapFrom(src => src.CurrencyId))
                .ForMember(dest => dest.CurrencyName, opt => opt.MapFrom(src => src.Currency.Name))
                .ForMember(dest => dest.CurrencyCode, opt => opt.MapFrom(src => src.Currency.Code))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.Balance, opt => opt.MapFrom(src => 
                    src.Transactions
                        .Where(t => t.CurrencyId == src.CurrencyId)
                        .Sum(t => t.TransactionType == TransactionType.Incoming ? t.Amount : -t.Amount)));
        }
    }
}
