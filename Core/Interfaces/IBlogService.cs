using Core.DTOs.Blog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IBlogService
    {
        Task<List<BlogCategoryDTO>> GetAllBlogCategoriesAsync();
        //Task<Core.Models.BlogCategory?> GetBlogCategoryByIdAsync(int id);
        Task<BlogCategoryDTO> CreateBlogCategoryAsync(CreateBlogCategoryDTO newCategory);
        //Task<bool> UpdateBlogCategoryAsync(int id, Core.DTOs.Blog.UpdateBlogCategoryDTO updatedCategory);
        Task<bool> DeleteBlogCategoryAsync(int id);


        Task<List<PostDTO>> GetAllPostsAsync();
        Task<PostDTO> GetPostByTitleAsync(string title);
        Task<PostDTO> GetPostsByCategoryIdAsync(int categoryId);
        Task<PostDTO> CreatePostAsync(CreatePostDTO newPost);
    }
}
