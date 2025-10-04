using Core.Enums;
using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.Blog
{
    public class PostDTO
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public required string Content { get; set; }
        public required string FeaturedImage { get; set; }
        public string? ImageAltText { get; set; }
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public string? Author { get; set; }
        public bool IsFeatured { get; set; }
        public DateOnly CreatedAt { get; set; }
        public int ContentLanguageId { get; set; }
        public ContentLanguage Language { get; set; }
    }
}
