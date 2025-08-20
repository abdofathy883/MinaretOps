using Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.Reports
{
    public class ClientTaskReportDTO
    {
        public int ClientId { get; set; }
        public string ClientName { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public ClientStatus Status { get; set; }
        public List<MonthlyTaskGroupDTO> MonthlyTasks { get; set; } = new();
    }
}
