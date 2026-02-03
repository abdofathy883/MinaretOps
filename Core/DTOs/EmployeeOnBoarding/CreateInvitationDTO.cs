using Core.Enums;

namespace Core.DTOs.EmployeeOnBoarding
{
    public class CreateInvitationDTO
    {
        public required string Email { get; set; }
        public UserRoles Role { get; set; }
    }
}
