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
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => $"{src.Category.Title}"));
        }
    }
}
