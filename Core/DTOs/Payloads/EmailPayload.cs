using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.Payloads
{
    public record EmailPayload
    (
        string To,
        string Subject,
        string Template,
        Dictionary<string, string> Replacements
    );
}
