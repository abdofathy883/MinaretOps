using Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Reporting
{
    public class EmployeeMonthlyIncidentsDTO
    {
        public KPIAspectType AspectType { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
