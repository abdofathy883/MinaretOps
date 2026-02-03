using Core.Enums;

namespace Core.Models
{
    public class EmployeeOnBoardingInvitation
    {
        public int Id { get; set; }
        public required string Email { get; set; }
        public UserRoles Role { get; set; }
        public required string InvitationToken { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ExpiresAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public InvitationStatus Status { get; set; } = InvitationStatus.Pending;
        public required string InvitedByUserId { get; set; }
        public ApplicationUser InvitedBy { get; set; } = default!;

        // Fields to be completed by invitee
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? City { get; set; }
        public string? Street { get; set; }
        public string? NID { get; set; }
        public string? PaymentNumber { get; set; }
        public DateOnly? DateOfHiring { get; set; }
        public string? Password { get; set; }
    }
}
