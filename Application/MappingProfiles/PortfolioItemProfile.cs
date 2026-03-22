using Application.DTOs.Portfolio.Item;
using AutoMapper;
using Core.Models.Portfolio;

namespace Application.MappingProfiles
{
    public class PortfolioItemProfile : Profile
    {
        public PortfolioItemProfile()
        {
            CreateMap<PortfolioItem, PortfolioItemDTO>();
            CreateMap<PortfolioTranslation, PortfolioTranslationDTO>();
        }
    }
}
