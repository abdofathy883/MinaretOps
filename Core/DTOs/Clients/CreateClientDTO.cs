using Core.DTOs.ClientServiceDTOs;
using Core.Enums;

namespace Core.DTOs.Clients
{
    public class CreateClientDTO
    {
        public required string Name { get; set; }
        public string? CompanyName { get; set; }
        public required string PersonalPhoneNumber { get; set; }
        public string? CompanyNumber { get; set; }
        public required string BusinessDescription { get; set; }
        public string? DriveLink { get; set; }
        public string? DiscordChannelId { get; set; }
        public List<CreateClientServiceDTO> ClientServices { get; set; } = new();
    }
}
