using AutoMapper;
using Core.DTOs.Blog;
using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.MappingProfiles
{
    public class PostProfile: Profile
    {
        public PostProfile()
        {
            CreateMap<Post, PostDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content))
                .ForMember(dest => dest.FeaturedImage, opt => opt.MapFrom(src => src.FeaturedImage))
                .ForMember(dest => dest.ImageAltText, opt => opt.MapFrom(src => src.ImageAltText))
                .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.Category))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => $"{src.Category.Title}"))
                .ForMember(dest => dest.Author, opt => opt.MapFrom(src => src.Author))
                .ForMember(dest => dest.IsFeatured, opt => opt.MapFrom(src => src.IsFeatured))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.ContentLanguageId, opt => opt.MapFrom(src => src.ContentLanguageId))
                .ForMember(dest => dest.Language, opt => opt.MapFrom(src => src.Language));

            CreateMap<BlogCategory, BlogCategoryDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.Language, opt => opt.MapFrom(src => src.Language))
                .ForMember(dest => dest.ContentLanguageId, opt => opt.MapFrom(src => src.ContentLanguageId))
                .ForMember(dest => dest.Posts, opt => opt.MapFrom(src => src.Posts));
        }
    }
}
