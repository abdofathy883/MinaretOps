namespace Core.DTOs.ContactFormDTOs
{
    public class NewContactFormEntryDTO
    {
        public required string FullName { get; set; }
        public string? CompanyName { get; set; }
        public string? Email { get; set; }
        public required string PhoneNumber { get; set; }
        public string? DesiredService { get; set; }
        public string? ProjectBrief { get; set; }
    }
}
