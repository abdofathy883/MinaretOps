using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.AuthDTOs
{
    public class LoginDTO
    {
        public required string PhoneNumber { get; set; }
        public required string Password { get; set; }
    }
}
