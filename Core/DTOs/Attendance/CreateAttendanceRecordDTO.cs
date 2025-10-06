using Core.Enums;
using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.Attendance
{
    public class CreateAttendanceRecordDTO
    {
        public required string EmployeeId { get; set; }
        public DateTime? ClockIn { get; set; }
        public DateTime? ClockOut { get; set; }
        public required string DeviceId { get; set; }
        public required string IpAddress { get; set; }
    }
}
