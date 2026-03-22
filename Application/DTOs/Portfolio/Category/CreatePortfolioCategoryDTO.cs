using Core.Enums;
using Core.Models.Portfolio;

namespace Application.DTOs.Portfolio.Category
{
    public class CreatePortfolioCategoryDTO
    {
        public List<CreatePortfolioCategoryTranslationDTO> Translations { get; set; } = new();
    }
}
