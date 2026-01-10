using Core.DTOs.Checkpoints;
using Core.DTOs.Tasks.TaskDTOs;
using Core.DTOs.Tasks.TaskGroupDTOs;

namespace Core.DTOs.ClientServiceDTOs
{
    public class ClientServiceDTO
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public string? ClientName { get; set; }
        public int ServiceId { get; set; }
        public string? ServiceTitle { get; set; }
        public decimal? ServiceCost { get; set; }
        //public DateTime StartDate { get; set; } = DateTime.UtcNow;
        //public DateTime? EndDate { get; set; }
        //public List<TaskDTO> TaskItems { get; set; } = new();
        public List<TaskGroupDTO> TaskGroups { get; set; } = new();
        public List<ClientServiceCheckpointDTO> ClientServiceCheckpoints { get; set; } = new();

        // Helper property to get all tasks across all groups
        //public List<TaskDTO> GetAllTasks() => TaskGroups.SelectMany(tg => tg.Tasks).ToList();
    }
}
