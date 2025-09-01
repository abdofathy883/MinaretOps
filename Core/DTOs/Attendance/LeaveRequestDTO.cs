using Core.Enums;
using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.Attendance
{
    public class LeaveRequestDTO
    {
        public int Id { get; set; }
        public required string EmployeeId { get; set; }
        public required string EmployeeName { get; set; }
        public DateTime Date { get; set; }
        public LeaveStatus Status { get; set; }
        public DateTime? ActionDate { get; set; }
    }
}
