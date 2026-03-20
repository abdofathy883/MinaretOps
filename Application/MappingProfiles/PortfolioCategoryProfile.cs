using Application.DTOs.Portfolio;
using AutoMapper;
using Core.Models;

namespace Application.MappingProfiles
{
    public class PortfolioCategoryProfile : Profile
    {
        public PortfolioCategoryProfile()
        {
            CreateMap<PortfolioCategory, PortfolioCategoryDTO>();
                //.ForMember(dest => dest.PortfolioItems, opt => opt.MapFrom(src => src.PortfolioItems));
        }
    }
}
