using Application.DTOs.Portfolio;
using Application.Interfaces;
using AutoMapper;
using Core.Models;
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

        public async Task<PortfolioItemDTO> Create(CreatePortfolioItemDTO createDTO)
        {
            var featuredImagURL = string.Empty;
            if (createDTO.ImageFile is not null)
            {
                var uploaded = await mediaUploadService.UploadImageWithPath(
                    createDTO.ImageFile,
                    $"Almnara_{createDTO.Title}"
                    );
                featuredImagURL = uploaded.Url;
            }

            var item = new PortfolioItem
            {
                Title = createDTO.Title,
                Description = createDTO.Description,
                ImageLink = featuredImagURL,
                ImageAltText = createDTO.ImageAltText,
                CategoryId = createDTO.CategoryId
            };
            await context.PortfolioItems.AddAsync(item);
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
                .Include(p => p.PortfolioCategory)
                .ToListAsync();
            return mapper.Map<List<PortfolioItemDTO>>(items);
        }

        public async Task<PortfolioItemDTO> GetById(int id)
        {
            var item = await context.PortfolioItems
                .Include(p => p.PortfolioCategory)
                .SingleAsync(p => p.Id == id)
                ?? throw new KeyNotFoundException($"No portfolio item found with id {id}");
            return mapper.Map<PortfolioItemDTO>(item);
        }
    }
}
