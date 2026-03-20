using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Leads
{
    public class LeadServicesDTO
    {
        public int ServiceId { get; set; }
        public string? ServiceTitle { get; set; }
        public int LeadId { get; set; }
        public string LeadName { get; set; }
    }
}
