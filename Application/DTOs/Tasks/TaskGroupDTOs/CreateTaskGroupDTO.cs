using Application.DTOs.Tasks.TaskDTOs;

namespace Application.DTOs.Tasks.TaskGroupDTOs
{
    public class CreateTaskGroupDTO
    {
        public int ClientId { get; set; }
        public int ServiceId { get; set; }
        public int ClientServiceId { get; set; }
        public List<CreateTaskDTO> Tasks { get; set; } = new();
    }
}
