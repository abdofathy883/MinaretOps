using Core.DTOs.Tasks;

namespace Core.DTOs.ClientServiceDTOs
{
    public class CreateClientServiceDTO
    {
        public int ClientId { get; set; }
        public int ServiceId { get; set; }
        //public DateTime StartDate { get; set; } = DateTime.UtcNow;
        //public DateTime? EndDate { get; set; }
        public List<CreateTaskGroupDTO> TaskGroups { get; set; } = new();
    }
}
