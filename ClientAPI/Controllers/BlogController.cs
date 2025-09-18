using Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ClientAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogController : ControllerBase
    {
        private readonly IBlogService blogService;
        public BlogController(IBlogService blog)
        {
            blogService = blog;
        }

        [HttpGet("categories")]
        public async Task<IActionResult> GetAllBlogCategoriesAsync()
        {
            var categories = await blogService.GetAllBlogCategoriesAsync();
            if (categories.Count > 0)
            {
                return Ok(categories);
            }
            return NotFound("No blog categories found.");
        }

        [HttpPost("category")]
        public async Task<IActionResult> CreateBlogCategoryAsync([FromBody] Core.DTOs.Blog.CreateBlogCategoryDTO newCategory)
        {
            if (newCategory is null)
                return BadRequest("Invalid category data.");
            try
            {
                var category = await blogService.CreateBlogCategoryAsync(newCategory);
                return Ok(category);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("category/{id}")]
        public async Task<IActionResult> DeleteBlogCategoryAsync(int id)
        {
            try
            {
                var result = await blogService.DeleteBlogCategoryAsync(id);
                if (result)
                    return Ok("Category deleted successfully.");
                return NotFound("Category not found.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


    }
}
