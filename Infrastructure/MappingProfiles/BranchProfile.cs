using AutoMapper;
using Core.DTOs.Branch;
using Core.DTOs.Vault;
using Core.Enums;
using Core.Models;

namespace Infrastructure.MappingProfiles
{
    public class BranchProfile : Profile
    {
        public BranchProfile()
        {
            CreateMap<Branch, BranchDTO>()
                .ForMember(dest => dest.Vault, opt => opt.MapFrom(src => src.Vault != null ? new VaultDTO
                {
                    Id = src.Vault.Id,
                    VaultType = src.Vault.VaultType,
                    BranchId = src.Vault.BranchId,
                    BranchName = src.Name,
                    CurrencyId = src.Vault.CurrencyId,
                    CurrencyName = src.Vault.Currency.Name ?? string.Empty,
                    CurrencyCode = src.Vault.Currency.Code ?? string.Empty,
                    CreatedAt = src.Vault.CreatedAt,
                    Balance = src.Vault.Transactions != null 
                        ? src.Vault.Transactions.Sum(t => t.TransactionType == TransactionType.Incoming ? t.Amount : -t.Amount)
                        : 0m
                } : null));
        }
    }
}
