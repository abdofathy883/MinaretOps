using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.Tasks
{
    public class CreateTaskGroupDTO
    {
        public int ClientServiceId { get; set; }
        public int Month { get; set; } // 1-12
        public int Year { get; set; }
        public string MonthLabel { get; set; } = string.Empty; // e.g., "August 2024"

        public List<CreateTaskDTO> Tasks { get; set; } = new();
    }
}
