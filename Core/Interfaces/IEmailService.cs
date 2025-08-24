namespace Core.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailWithTemplateAsync(string to, string subject, string templateName, Dictionary<string, string> replacements);
    }
}
