using Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.Blog
{
    public class BlogCategoryDTO
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public int ContentLanguageId { get; set; }
        public ContentLanguage Language { get; set; }
        public List<PostDTO> Posts { get; set; } = new();
    }
}
