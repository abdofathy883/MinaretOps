using Application.DTOs.Portfolio.Item;
using Application.Interfaces;
using AutoMapper;
using Core.Models;
using Core.Models.Portfolio;
using Infrastructure.Persistance;
using Infrastructure.Services.MediaUploads;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services.Portfolio
{
    public class PortfolioService : IPortfolioService
    {
        private readonly MinaretOpsDbContext context;
        private readonly MediaUploadService mediaUploadService;
        private readonly IMapper mapper;

        public PortfolioService(MinaretOpsDbContext context, MediaUploadService mediaUploadService, IMapper mapper)
        {
            this.context = context;
            this.mediaUploadService = mediaUploadService;
            this.mapper = mapper;
        }

        public async Task<PortfolioItemDTO> Create(CreatePortfolioItemDTO createDTO, int? itemId = null)
        {
            PortfolioItem item;

            if (itemId.HasValue)
            {
                item = await context.PortfolioItems
                    .Include(p => p.Translations)
                    .FirstOrDefaultAsync(p => p.Id == itemId.Value)
                    ?? throw new KeyNotFoundException($"No portfolio item found with id {itemId}");

                // Add translations
                foreach (var t in createDTO.Translations)
                {
                    if (!item.Translations.Any(tr => tr.LanguageCode == t.LanguageCode))
                    {
                        item.Translations.Add(new PortfolioTranslation
                        {
                            LanguageCode = t.LanguageCode,
                            Title = t.Title,
                            Description = t.Description,
                            ImageAltText = t.ImageAltText,
                            Status = t.Status
                        });
                    }
                }
            }
            else
            {
                var featuredImagURL = string.Empty;
                if (createDTO.ImageFile is not null)
                {
                    var uploaded = await mediaUploadService.UploadImageWithPath(
                        createDTO.ImageFile,
                        $"Almnara_{createDTO.Slug}"
                        );
                    featuredImagURL = uploaded.Url;
                }

                item = new PortfolioItem
                {
                    Slug = createDTO.Slug,
                    ImageLink = featuredImagURL,
                    CategoryId = createDTO.CategoryId,
                    PublishedAt = DateTime.UtcNow,
                    Translations = createDTO.Translations.Select(t => new PortfolioTranslation
                    {
                        LanguageCode = t.LanguageCode,
                        Title = t.Title,
                        Description = t.Description,
                        ImageAltText = t.ImageAltText,
                        Status = t.Status
                    }).ToList()
                };
                await context.PortfolioItems.AddAsync(item);
            }

            await context.SaveChangesAsync();
            return mapper.Map<PortfolioItemDTO>(item);
        }

        public async Task<bool> Delete(int id)
        {
            var item = await context.PortfolioItems.FindAsync(id)
                ?? throw new KeyNotFoundException($"No portfolio item found with id {id}");
            context.PortfolioItems.Remove(item);
            return await context.SaveChangesAsync() > 0;
        }

        public async Task<List<PortfolioItemDTO>> GetAll()
        {
            var items = await context.PortfolioItems
                .Include(p => p.Translations)
                //.Include(p => p.PortfolioCategory)
                .ToListAsync();
            return mapper.Map<List<PortfolioItemDTO>>(items);
        }

        public async Task<PortfolioItemDTO> GetById(int id)
        {
            var item = await context.PortfolioItems
                .Include(p => p.Translations)
                //.Include(p => p.PortfolioCategory)
                .SingleAsync(p => p.Id == id)
                ?? throw new KeyNotFoundException($"No portfolio item found with id {id}");
            return mapper.Map<PortfolioItemDTO>(item);
        }

        public async Task<PortfolioItemDTO> GetBySlug(string slug)
        {
            var item = await context.PortfolioItems
                .Include(p => p.Translations)
                .SingleOrDefaultAsync(p => p.Slug == slug)
                ?? throw new KeyNotFoundException($"No portfolio item found with slug {slug}");
            return mapper.Map<PortfolioItemDTO>(item);
        }
    }
}
