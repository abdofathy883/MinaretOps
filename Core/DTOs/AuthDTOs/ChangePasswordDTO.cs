namespace Core.DTOs.AuthDTOs
{
    public class ChangePasswordDTO
    {
        public required string Id { get; set; }
        public required string OldPassword { get; set; }
        public required string NewPassword { get; set; }
        public required string ConfirmNewPassword { get; set; }
    }
}
