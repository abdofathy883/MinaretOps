using AutoMapper;
using Core.DTOs.Blog;
using Core.Interfaces;
using Core.Models;
using Infrastructure.Data;
using Infrastructure.Services.MediaUploads;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services.Blog
{
    public class BlogService : IBlogService
    {
        private readonly MinaretOpsDbContext context;
        private readonly MediaUploadService mediaUploadService;
        private readonly IMapper mapper;
        public BlogService(
            MinaretOpsDbContext minaret,
            MediaUploadService media,
            IMapper _mapper
            )
        {
            context = minaret;
            mediaUploadService = media;
            mapper = _mapper;
        }

        public async Task<BlogCategoryDTO> CreateBlogCategoryAsync(CreateBlogCategoryDTO newCategory)
        {
            if (newCategory is null)
                throw new Exception("Invalid category data.");

            var existingCategory = context.BlogCategories
                .FirstOrDefault(c => c.Title == newCategory.Title);

            if (existingCategory is not null)
                throw new Exception("Category with the same title already exists.");

            var cat = new BlogCategory
            {
                Title = newCategory.Title
            };

            await context.BlogCategories.AddAsync(cat);
            await context.SaveChangesAsync();
            return mapper.Map<BlogCategoryDTO>(cat);
        }

        public Task<PostDTO> CreatePostAsync(CreatePostDTO newPost)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> DeleteBlogCategoryAsync(int id)
        {
            var category = context.BlogCategories.FirstOrDefault(c => c.Id == id);
            if (category is null)
                throw new Exception("Category not found.");
            context.BlogCategories.Remove(category);
            return await context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeletePostAsync(int id)
        {
            var post = context.BlogPosts.FirstOrDefault(p => p.Id == id);
            if (post is null)
                throw new Exception("Post not found.");

            context.BlogPosts.Remove(post);
            return await context.SaveChangesAsync() > 0;
        }

        public async Task<List<BlogCategoryDTO>> GetAllBlogCategoriesAsync()
        {
            var categories = await context.BlogCategories.ToListAsync();
            return mapper.Map<List<BlogCategoryDTO>>(categories);
        }

        public Task<List<PostDTO>> GetAllPostsAsync()
        {
            var posts = context.BlogPosts
                .Include(p => p.Category)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
            return mapper.Map<Task<List<PostDTO>>>(posts);
        }

        public async Task<PostDTO> GetPostByTitleAsync(string title)
        {
            var post = await context.BlogPosts
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Title == title);

            return mapper.Map<PostDTO>(post);
        }
    }
}
