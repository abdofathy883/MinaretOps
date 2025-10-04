using Core.Enums;
using Microsoft.AspNetCore.Http;

namespace Core.DTOs.Blog
{
    public class CreateBlogCategoryDTO
    {
        public required string Title { get; set; }
        public int? ContentLanguageId { get; set; }
        public ContentLanguage Language { get; set; }
    }
}
