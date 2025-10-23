using System;

namespace Core.DTOs.Tasks
{
    public class TaskFilterDTO
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string? EmployeeId { get; set; }
        public int? ClientId { get; set; }
        public int? Status { get; set; }
        public string? Priority { get; set; }
        public string? OnDeadline { get; set; }
        public string? Team { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
