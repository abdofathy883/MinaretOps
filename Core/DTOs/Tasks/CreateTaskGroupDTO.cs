namespace Core.DTOs.Tasks
{
    public class CreateTaskGroupDTO
    {
        public int ClientServiceId { get; set; }
        public List<CreateTaskDTO> Tasks { get; set; } = new();
    }
}
