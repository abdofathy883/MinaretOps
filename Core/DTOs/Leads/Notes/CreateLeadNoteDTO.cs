using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.Leads.Notes
{
    public class CreateLeadNoteDTO
    {
        public string Note { get; set; }
        public required string CreatedById { get; set; }
        public int LeadId { get; set; }
    }
}
