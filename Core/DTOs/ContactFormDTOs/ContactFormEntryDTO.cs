namespace Core.DTOs.ContactFormDTOs
{
    public class ContactFormEntryDTO
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string CompanyName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string DesiredService { get; set; }
        public string ProjectBrief { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}
