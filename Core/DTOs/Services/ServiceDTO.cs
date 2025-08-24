using Core.DTOs.ClientServiceDTOs;

namespace Core.DTOs.Services
{
    public class ServiceDTO
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public string? Description { get; set; }
        public DateOnly CreatedAt { get; set; }
        public bool IsDeleted { get; set; }

        public List<ClientServiceDTO> ClientServices { get; set; } = new();
    }
}
