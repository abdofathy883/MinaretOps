using Application.DTOs.Portfolio;
using Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ClientAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PortfolioController : ControllerBase
    {
        private readonly IPortfolioService _portfolioService;
        private readonly IPortfolioCategoryService _portfolioCategoryService;

        public PortfolioController(IPortfolioService portfolioService, IPortfolioCategoryService portfolioCategoryService)
        {
             _portfolioService = portfolioService;
            _portfolioCategoryService = portfolioCategoryService;
        }

        [HttpGet("categories")]
        public async Task<IActionResult> GetAllCategories()
        {
            var categories = await _portfolioCategoryService.GetAll();
            return Ok(categories);
        }
        [HttpGet("categories/{id}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            var category = await _portfolioCategoryService.GetById(id);
            return Ok(category);
        }
        [HttpPost("categories")]
        public async Task<IActionResult> CreateCategory(CreatePortfolioCategoryDTO createCategory)
        {
            var category = await _portfolioCategoryService.Create(createCategory);
            return Ok(category);
        }


        [HttpGet("items")]
        public async Task<IActionResult> GetAllItems()
        {
            var items = await _portfolioService.GetAll();
            return Ok(items);
        }

        [HttpGet("items/{id}")]
        public async Task<IActionResult> GetItemById(int id)
        {
            var item = await _portfolioService.GetById(id);
            return Ok(item);
        }

        [HttpPost("items")]
        public async Task<IActionResult> CreateItem(CreatePortfolioItemDTO createItem)
        {
            var item = await _portfolioService.Create(createItem);
            return Ok(item);
        }
    }
}
