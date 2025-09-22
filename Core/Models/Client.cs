using Core.Enums;
using Core.Interfaces;

namespace Core.Models
{
    public class Client: IAuditable
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? CompanyName { get; set; }
        public required string PersonalPhoneNumber { get; set; }
        public string? CompanyNumber { get; set; }
        public required string BusinessDescription { get; set; }
        public string? DriveLink { get; set; }
        public string? DiscordChannelId { get; set; }
        public ClientStatus Status { get; set; } = ClientStatus.Active;
        public string? StatusNotes { get; set; }
        public DateOnly CreatedAt { get; set; } = DateOnly.FromDateTime(DateTime.Today);
        public DateOnly? UpdatedAt { get; set; }
        public List<ClientService> ClientServices { get; set; } = new();
    }
}
