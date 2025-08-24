using Core.Enums;

namespace Core.DTOs.Clients
{
    public class LightWieghtClientDTO
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? CompanyName { get; set; }
        public ClientStatus Status { get; set; }
        public int ServiceId { get; set; }
        public string ServiceTitle { get; set; }
    }
}
