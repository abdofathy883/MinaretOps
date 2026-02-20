using Core.Enums.Auth_Attendance;
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
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public LeaveStatus Status { get; set; }
        public DateTime? ActionDate { get; set; }
        public DateTime RequestDate { get; set; }
        public LeaveRequestType Type { get; set; }
        public required string Reason { get; set; }
        public string? ProofFile { get; set; }
    }
}
