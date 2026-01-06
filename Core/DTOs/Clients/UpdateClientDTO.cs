using Core.Enums;

namespace Core.DTOs.Clients
{
    public class UpdateClientDTO
    {
        public string? Name { get; set; }
        public string? CompanyName { get; set; }
        public string? PersonalPhoneNumber { get; set; }
        public string? CompanyNumber { get; set; }
        public string? Email { get; set; }
        public string? BusinessDescription { get; set; }
        public string? DriveLink { get; set; }
        public BusinessType BusinessType { get; set; }
        public string BusinessActivity { get; set; }
        public string? CommercialRegisterNumber { get; set; }
        public string? TaxCardNumber { get; set; }
        public string Country { get; set; }
        public string AccountManagerId { get; set; }
        public string? DiscordChannelId { get; set; }
        public ClientStatus Status { get; set; }
        public string? StatusNotes { get; set; }
    }
}
