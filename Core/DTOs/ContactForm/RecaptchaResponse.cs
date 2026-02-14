namespace Core.DTOs.ContactForm
{
    public class RecaptchaResponse
    {
        public bool Success { get; set; }
        public double Score { get; set; }
        public string Action { get; set; } = null!;
    }
}
