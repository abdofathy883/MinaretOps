using Core.Enums;
using Core.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.Blog
{
    public class CreatePostDTO
    {
        public required string Title { get; set; }
        public required string Content { get; set; }
        public required IFormFile FeaturedImage { get; set; }
        public string? ImageAltText { get; set; }
        public int CategoryId { get; set; }
        public string? Author { get; set; }
        public bool IsFeatured { get; set; }
        public int? ContentLanguageId { get; set; }
        public ContentLanguage Language { get; set; }

    }
}
