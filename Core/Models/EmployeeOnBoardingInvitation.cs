using Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class EmployeeOnBoardingInvitation
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(100)]
        public required string Email { get; set; }
        [Required]
        public UserRoles Role { get; set; }
        [Required]
        [MaxLength(500)]
        public required string InvitationToken { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ExpiresAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public InvitationStatus Status { get; set; } = InvitationStatus.Pending;
        public string? InvitedByUserId { get; set; }
        public ApplicationUser? InvitedBy { get; set; }

        // Fields to be completed by invitee
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? City { get; set; }
        public string? Street { get; set; }
        public string? NID { get; set; }
        public string? PaymentNumber { get; set; }
        public DateOnly? DateOfHiring { get; set; }
        public string? ProfilePicture { get; set; }
        public string? JobTitle { get; set; }
        public string? Bio { get; set; }
    }
}
