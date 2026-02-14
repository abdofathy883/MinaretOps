namespace Core.DTOs.ContactForm
{
    public class NewEntryDTO
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Message { get; set; }
        public string RecaptchaToken { get; set; }
        public string? Website { get; set; }
    }
}
