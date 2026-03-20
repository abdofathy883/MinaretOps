using Application.DTOs.Portfolio;
using AutoMapper;
using Core.Models;

namespace Application.MappingProfiles
{
    public class PortfolioItemProfile : Profile
    {
        public PortfolioItemProfile()
        {
            CreateMap<PortfolioItem, PortfolioItemDTO>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.PortfolioCategory != null ? src.PortfolioCategory.Title : null));
        }
    }
}
