using Core.DTOs.ClientServiceDTOs;
using Core.Enums;

namespace Core.DTOs.Clients
{
    public class ClientDTO
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? CompanyName { get; set; }
        public required string PersonalPhoneNumber { get; set; }
        public string? CompanyNumber { get; set; }
        public required string BusinessDescription { get; set; }
        public string? DriveLink { get; set; }
        public string? DiscordChannelId { get; set; }
        public ClientStatus Status { get; set; }
        public string? StatusNotes { get; set; }
        public DateOnly CreatedAt { get; set; }
        public DateOnly? UpdatedAt { get; set; }
        public List<ClientServiceDTO> ClientServices { get; set; } = new();
    }
}
