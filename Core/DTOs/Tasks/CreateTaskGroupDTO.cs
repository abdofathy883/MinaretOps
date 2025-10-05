namespace Core.DTOs.Tasks
{
    public class CreateTaskGroupDTO
    {
        public int ClientId { get; set; }
        public int ServiceId { get; set; }
        public int ClientServiceId { get; set; }
        public List<CreateTaskDTO> Tasks { get; set; } = new();
    }
}
