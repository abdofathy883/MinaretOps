using Application.Interfaces;
using AutoMapper;
using Application.DTOs.Portfolio;
using Infrastructure.Persistance;
using Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services.Portfolio
{
    public class PortfolioCategoryService : IPortfolioCategoryService
    {
        private readonly MinaretOpsDbContext context;
        private readonly IMapper mapper;

        public PortfolioCategoryService(MinaretOpsDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<PortfolioCategoryDTO> Create(CreatePortfolioCategoryDTO createCategory)
        {
            var newCat = new PortfolioCategory {
                Title = createCategory.Title,
                Description = createCategory.Description ?? string.Empty
            };

            await context.PortfolioCategories.AddAsync(newCat);
            await context.SaveChangesAsync();
            return mapper.Map<PortfolioCategoryDTO>(newCat);
        }

        public async Task<List<PortfolioCategoryDTO>> GetAll()
        {
            var categories = await context.PortfolioCategories
                .Include(c => c.PortfolioItems)
                .ToListAsync();
            return mapper.Map<List<PortfolioCategoryDTO>>(categories);
        }

        public async Task<PortfolioCategoryDTO> GetById(int id)
        {
            var category = await context.PortfolioCategories
                .Include(c => c.PortfolioItems)
                .SingleAsync(c => c.Id == id);

            return mapper.Map<PortfolioCategoryDTO>(category);
        }
    }
}
