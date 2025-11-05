using Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.EmployeeOnBoarding
{
    public class InvitationDTO
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public UserRoles Role { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public InvitationStatus Status { get; set; }
        public string? InvitedByUserName { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
    }
}
