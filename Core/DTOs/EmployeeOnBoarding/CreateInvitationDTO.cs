using Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.EmployeeOnBoarding
{
    public class CreateInvitationDTO
    {
        public required string Email { get; set; }
        public UserRoles Role { get; set; }
    }
}
