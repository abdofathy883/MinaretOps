using Core.Enums;
using Microsoft.AspNetCore.Http;

namespace Core.DTOs.Tasks.TaskDTOs
{
    public class CreateTaskDTO
    {
        public required string Title { get; set; }
        public required TaskType TaskType { get; set; }
        public string? Description { get; set; }
        public int ClientServiceId { get; set; }
        public DateTime Deadline { get; set; }
        public string Priority { get; set; } = "عادي";
        public string? Refrence { get; set; }
        public string? EmployeeId { get; set; }
        public int TaskGroupId { get; set; }
        //public IFormFile? ReferenceFile { get; set; }
    }
}
