using Core.Enums;
using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.KPI
{
    public class IncedintDTO
    {
        public int Id { get; set; }
        public string EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public KPIAspectType Aspect { get; set; }
        public DateTime TimeStamp { get; set; }
        public int PenaltyPercentage { get; set; } = 10; // currently fixed
        public string? Description { get; set; }
        public string? EvidenceURL { get; set; }

    }
}
