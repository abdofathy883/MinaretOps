using Core.DTOs.Seo;

namespace Core.Interfaces
{
    public interface ISeoService
    {
        Task<SeoContentDTO> GetContentByRoute(string route, string language = "en");
        Task<SeoContentDTO> CreateSeoContent(CreateSeoContentDTO newContent);
        Task<SeoContentDTO> UpdateSeoContent(CreateSeoContentDTO content);
    }
}
