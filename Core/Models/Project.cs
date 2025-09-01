using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class Project
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public required string Content { get; set; }
        public required string FeaturedImage { get; set; }
        public string? ImageAltText { get; set; }
        public int CategoryId { get; set; }
        public ProjectCategory Category { get; set; } = default!;
    }
}
