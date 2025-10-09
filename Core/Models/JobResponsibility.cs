using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class JobResponsibility
    {
        public int Id { get; set; }
        public int JobDescriptionId { get; set; }
        public JobDescription JobDescription { get; set; } = default!;
        public required string Text { get; set; }
    }
}
