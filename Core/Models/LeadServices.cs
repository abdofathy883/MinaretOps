using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class LeadServices
    {
        public int Id { get; set; }
        public int ServiceId { get; set; }
        public Service Service { get; set; } = default!;

        public int LeadId { get; set; }
        public SalesLead Lead { get; set; } = default!;
    }
}
