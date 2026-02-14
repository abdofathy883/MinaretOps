using AutoMapper;
using Core.DTOs.Seo;
using Core.Interfaces;
using Infrastructure.Data;
using Infrastructure.Services.MediaUploads;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services.Seo
{
    public class SeoService : ISeoService
    {
        private readonly MinaretOpsDbContext context;
        private readonly MediaUploadService mediaUploadService;
        private readonly IMapper mapper;

        public SeoService(MinaretOpsDbContext context, MediaUploadService mediaUploadService, IMapper mapper)
        {
            this.context = context;
            this.mediaUploadService = mediaUploadService;
            this.mapper = mapper;
        }

        public async Task<SeoContentDTO> CreateSeoContent(CreateSeoContentDTO newContent)
        {
            var seoContent = new Core.Models.SeoContent
            {
                Route = newContent.Route,
                Language = newContent.Language,
                Title = newContent.Title,
                Description = newContent.Description,
                Keywords = newContent.Keywords,
                OgTitle = newContent.OgTitle,
                OgDescription = newContent.OgDescription,
                CanonicalUrl = newContent.CanonicalUrl,
                Robots = newContent.Robots,
                CreatedAt = DateTime.UtcNow
            };

            if (newContent.OgImage != null)
            {
                var uploadResult = await mediaUploadService.UploadImageWithPath(newContent.OgImage, "SEO_" + newContent.Route.Replace("/", "_"));
                seoContent.OgImage = uploadResult.Url;
            }

            context.SeoContents.Add(seoContent);
            await context.SaveChangesAsync();

            return mapper.Map<SeoContentDTO>(seoContent);
        }

        public async Task<SeoContentDTO> GetContentByRoute(string route, string language = "en")
        {
            var sanitizedRoute = route.StartsWith("/") ? route : "/" + route;
            if (sanitizedRoute == "//") sanitizedRoute = "/";

            var content = await context.SeoContents
                .FirstOrDefaultAsync(x => x.Route == sanitizedRoute && x.Language == language)
                ?? throw new KeyNotFoundException();

            return mapper.Map<SeoContentDTO>(content);
        }

        public async Task<SeoContentDTO> UpdateSeoContent(CreateSeoContentDTO content)
        {
            var existingContent = await context.SeoContents
                .FirstOrDefaultAsync(x => x.Route == content.Route && x.Language == content.Language);

            if (existingContent == null)
            {
                return await CreateSeoContent(content);
            }

            existingContent.Title = content.Title;
            existingContent.Description = content.Description;
            existingContent.Keywords = content.Keywords;
            existingContent.OgTitle = content.OgTitle;
            existingContent.OgDescription = content.OgDescription;
            existingContent.CanonicalUrl = content.CanonicalUrl;
            existingContent.Robots = content.Robots;
            existingContent.UpdatedAt = DateTime.UtcNow;

            if (content.OgImage != null)
            {
                var uploadResult = await mediaUploadService.UploadImageWithPath(content.OgImage, "SEO_" + content.Route.Replace("/", "_"));
                existingContent.OgImage = uploadResult.Url;
            }

            await context.SaveChangesAsync();
            return mapper.Map<SeoContentDTO>(existingContent);
        }
    }
}
