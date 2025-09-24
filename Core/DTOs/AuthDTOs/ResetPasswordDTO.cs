using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.AuthDTOs
{
    public class ResetPasswordDTO
    {
        public required string UserId { get; set; }
        public required string Token { get; set; }
        public required string NewPassword { get; set; }
    }
}
