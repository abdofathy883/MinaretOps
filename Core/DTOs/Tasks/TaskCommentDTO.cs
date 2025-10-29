using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.Tasks
{
    public class TaskCommentDTO
    {
        public int Id { get; set; }
        public required string Comment { get; set; }
        public int TaskId { get; set; }
        public string? EmployeeId { get; set; }
        public string? EmployeeName { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
