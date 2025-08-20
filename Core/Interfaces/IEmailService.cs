using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailWithTemplateAsync(string to, string subject, string templateName, Dictionary<string, string> replacements);
    }
}
