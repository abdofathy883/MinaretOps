namespace Core.DTOs.Tasks
{
    public class CreateTaskGroupDTO
    {
        public int ClientServiceId { get; set; }
        public List<CreateTaskForGroupDTO> Tasks { get; set; } = new();
    }
}
