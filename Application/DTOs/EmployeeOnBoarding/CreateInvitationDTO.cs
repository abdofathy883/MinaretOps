using Core.Enums.Auth_Attendance;

namespace Application.DTOs.EmployeeOnBoarding
{
    public class CreateInvitationDTO
    {
        public required string Email { get; set; }
        public UserRoles Role { get; set; }
    }
}
