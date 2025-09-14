using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class Project
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [MinLength(10)]
        [MaxLength(100)]
        public required string Title { get; set; }
        [Required]
        [MinLength(10)]
        [MaxLength(4000)]
        public required string Content { get; set; }
        [Required]
        public required string FeaturedImage { get; set; }
        public string? ImageAltText { get; set; }
        public int CategoryId { get; set; }
        public ProjectCategory Category { get; set; } = default!;
    }
}
