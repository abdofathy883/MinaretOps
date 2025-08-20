using Core.Enums;

namespace Core.DTOs.Clients
{
    public class UpdateClientDTO
    {
        public string? Name { get; set; }
        public string? CompanyName { get; set; }
        public string? PersonalPhoneNumber { get; set; }
        public string? CompanyNumber { get; set; }
        public string? BusinessDescription { get; set; }
        public string? DriveLink { get; set; }
        public ClientStatus Status { get; set; }
        public string? StatusNotes { get; set; }
    }
}
