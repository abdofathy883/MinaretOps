using Core.Interfaces;
using Core.Settings;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Infrastructure.Services.Email
{
    public class EmailService: IEmailService
    {
        private readonly IOptions<EmailSettings> emailSettings;
        public EmailService(IOptions<EmailSettings> options)
        {
            emailSettings = options;
        }
        public async Task SendEmailWithTemplateAsync(string to, string subject, string templateName, Dictionary<string, string> replacements)
        {
            var fromEmail = emailSettings.Value.FromEmail;
            var SMTPServer = emailSettings.Value.SMTP;
            var SMTPPort = emailSettings.Value.PORT;
            var emailPassword = emailSettings.Value.AppPassword;

            var templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "EmailTemplates", $"{templateName}.html");

            // Fallback: if not found in base directory, try to find it relative to the executing assembly
            if (!File.Exists(templatePath))
            {
                var assemblyLocation = System.Reflection.Assembly.GetExecutingAssembly().Location;
                var assemblyDirectory = Path.GetDirectoryName(assemblyLocation);
                templatePath = Path.Combine(assemblyDirectory!, "EmailTemplates", $"{templateName}.html");
            }

            if (!File.Exists(templatePath))
            {
                throw new FileNotFoundException($"Email template '{templateName}.html' not found. Searched in: {templatePath}");
            }
            //var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplates", $"{templateName}.html");
            var htmlbody = await File.ReadAllTextAsync(templatePath);

            // Handle both formats: {{Key}} and Key
            foreach (var pair in replacements)
            {
                // Replace {{Key}} format
                htmlbody = htmlbody.Replace($"{{{{{pair.Key}}}}}", pair.Value);
                // Replace Key format (without braces) as fallback
                htmlbody = htmlbody.Replace(pair.Key, pair.Value);
            }

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("The Minaret Agency", fromEmail));
            message.To.Add(new MailboxAddress("", to));
            message.Subject = subject;
            message.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = htmlbody
            };

            using (var client = new MailKit.Net.Smtp.SmtpClient())
            {
                await client.ConnectAsync(SMTPServer, SMTPPort, SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(fromEmail, emailPassword);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
        }
    }
}
