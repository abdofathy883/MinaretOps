using Application.Interfaces;
using AutoMapper;
using Infrastructure.Persistance;
using Core.Models;
using Microsoft.EntityFrameworkCore;
using Application.DTOs.Portfolio.Category;
using Core.Models.Portfolio;

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

        public async Task<PortfolioCategoryDTO> Create(CreatePortfolioCategoryDTO createCategory, int? categoryId)
        {
            PortfolioCategory category;

            if (categoryId.HasValue)
            {
                category = await context.PortfolioCategories
                    .Include(c => c.Translations)
                    .FirstOrDefaultAsync(c => c.Id == categoryId.Value)
                    ?? throw new Exception("Category not found");
                
                foreach (var t in createCategory.Translations)
                {
                    if (!category.Translations.Any(tr => tr.LanguageCode == t.LanguageCode))
                    {
                        category.Translations.Add(new PortfolioCategoryTranslation
                        {
                            LanguageCode = t.LanguageCode,
                            Title = t.Title,
                            Description = t.Description ?? string.Empty
                        });
                    }
                }
            }
            else
            {
                category = new PortfolioCategory
                {
                    Translations = createCategory.Translations.Select(t => new PortfolioCategoryTranslation
                    {
                        LanguageCode = t.LanguageCode,
                        Title = t.Title,
                        Description = t.Description ?? string.Empty
                    }).ToList()
                };
                await context.PortfolioCategories.AddAsync(category);
            }

            await context.SaveChangesAsync();
            return mapper.Map<PortfolioCategoryDTO>(category);
        }

        public async Task<bool> Delete(int id)
        {
            var category = await context.PortfolioCategories.FindAsync(id)
                ?? throw new KeyNotFoundException($"No portfolio category found with id {id}");
            context.PortfolioCategories.Remove(category);
            return await context.SaveChangesAsync() > 0;
        }

        public async Task<List<PortfolioCategoryDTO>> GetAll()
        {
            var categories = await context.PortfolioCategories
                .Include(c => c.Translations)
                .Include(c => c.PortfolioItems)
                .ToListAsync();
            return mapper.Map<List<PortfolioCategoryDTO>>(categories);
        }

        public async Task<PortfolioCategoryDTO> GetById(int id)
        {
            var category = await context.PortfolioCategories
                .Include(c => c.Translations)
                .Include(c => c.PortfolioItems)
                .SingleAsync(c => c.Id == id);

            return mapper.Map<PortfolioCategoryDTO>(category);
        }
    }
}
