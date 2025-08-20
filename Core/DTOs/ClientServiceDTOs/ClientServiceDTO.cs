using Core.DTOs.Tasks;
using Core.Models;

namespace Core.DTOs.ClientServiceDTOs
{
    public class ClientServiceDTO
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public string ClientName { get; set; }
        public int ServiceId { get; set; }
        public string ServiceTitle { get; set; }
        public DateTime StartDate { get; set; } = DateTime.UtcNow;
        public DateTime? EndDate { get; set; }

        public List<TaskDTO> TaskItems { get; set; } = new();






        // Changed from direct TaskItems to TaskGroups for month-based organization
        public List<TaskGroupDTO> TaskGroups { get; set; } = new();

        // Helper property to get all tasks across all groups
        public List<TaskDTO> GetAllTasks() => TaskGroups.SelectMany(tg => tg.Tasks).ToList();
    }
}
