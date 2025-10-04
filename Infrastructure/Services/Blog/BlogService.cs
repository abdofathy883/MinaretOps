using AutoMapper;
using Core.DTOs.Blog;
using Core.Interfaces;
using Core.Models;
using Infrastructure.Data;
using Infrastructure.Exceptions;
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
            var existingCategory = context.BlogCategories
                .FirstOrDefault(c => c.Title == newCategory.Title);

            if (existingCategory is not null)
                throw new Exception("Category with the same title already exists.");

            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                int contentId;
                if (newCategory.ContentLanguageId.HasValue && newCategory.ContentLanguageId.Value != 0)
                {
                    contentId = newCategory.ContentLanguageId.Value;
                }
                else
                {
                    contentId = await context.BlogCategories.AnyAsync()
                        ? await context.BlogCategories.MaxAsync(c => c.ContentLanguageId) + 1
                        : 1;
                }
                var cat = new BlogCategory
                {
                    Title = newCategory.Title,
                    Language = newCategory.Language,
                    ContentLanguageId = contentId
                };

                await context.BlogCategories.AddAsync(cat);
                await context.SaveChangesAsync();
                await transaction.CommitAsync();
                return mapper.Map<BlogCategoryDTO>(cat);
            }
            catch(Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception(ex.Message);
            }
        }

        public async Task<PostDTO> CreatePostAsync(CreatePostDTO newPost)
        {
            var existingPost = await context.BlogPosts
                .FirstOrDefaultAsync(p => p.Title == newPost.Title);

            if (existingPost is not null)
                throw new AlreadyExistObjectException("مقال بهذا العنوان موجود بالفعل");

            var imageUrl = await mediaUploadService.UploadImageWithPath(newPost.FeaturedImage, newPost.Title);

            var post = new Post
            {
                Title = newPost.Title,
                Content = newPost.Content,
                FeaturedImage = imageUrl.Url,
                ImageAltText = newPost.ImageAltText ?? string.Empty,
                CategoryId = newPost.CategoryId,
                Author = newPost.Author,
                IsFeatured = newPost.IsFeatured,
            };

            await context.AddAsync(post);
            await context.SaveChangesAsync();
            return mapper.Map<PostDTO>(post);
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
