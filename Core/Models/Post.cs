using Core.Enums;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class Post: IAuditable
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required] [MaxLength(100)] [MinLength(3)]
        public required string Title { get; set; }
        [Required] [MaxLength(3000)] [MinLength(10)]
        public required string Content { get; set; }
        [Required]
        public required string FeaturedImage { get; set; }
        public string? ImageAltText { get; set; }
        public string? Author { get; set; }
        public bool IsFeatured { get; set; } = false;
        public DateOnly CreatedAt { get; set; }
        public DateOnly? UpdatedAt { get; set; }
        public int CategoryId { get; set; }
        public BlogCategory Category { get; set; } = default!;
        public int ContentLanguageId { get; set; }
        public ContentLanguage Language { get; set; }
    }
}
