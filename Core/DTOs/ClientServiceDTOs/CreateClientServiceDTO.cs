using Core.DTOs.Tasks.TaskGroupDTOs;

namespace Core.DTOs.ClientServiceDTOs
{
    public class CreateClientServiceDTO
    {
        public int ServiceId { get; set; }
        public List<int> SelectedCheckpointIds { get; set; } = new();
        public List<CreateTaskGroupDTO> TaskGroups { get; set; } = new();
    }
}
