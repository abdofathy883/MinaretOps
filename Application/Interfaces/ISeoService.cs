using Application.DTOs.Seo;

namespace Application.Interfaces
{
    public interface ISeoService
    {
        Task<SeoContentDTO> GetContentByRoute(string route, string language);
        Task<SeoContentDTO> CreateSeoContent(CreateSeoContentDTO newContent);
        Task<SeoContentDTO> UpdateSeoContent(CreateSeoContentDTO content);
    }
}
