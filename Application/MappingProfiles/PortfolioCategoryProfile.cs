using Application.DTOs.Portfolio.Category;
using AutoMapper;
using Core.Models.Portfolio;

namespace Application.MappingProfiles
{
    public class PortfolioCategoryProfile : Profile
    {
        public PortfolioCategoryProfile()
        {
            CreateMap<PortfolioCategory, PortfolioCategoryDTO>();
            CreateMap<PortfolioCategoryTranslation, PortfolioCategoryTranslationDTO>();
        }
    }
}
