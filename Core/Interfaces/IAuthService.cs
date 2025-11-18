using Core.DTOs.AuthDTOs;

namespace Core.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDTO> RegisterUserAsync(RegisterUserDTO newUser);
        Task<AuthResponseDTO> LoginAsync(LoginDTO login);
        Task<AuthResponseDTO> UpdateUserAsync(UpdateUserDTO updatedUser);
        Task<AuthResponseDTO> ChangePasswordAsync(ChangePasswordDTO passwordDTO);
        Task<bool> ToggleVisibilityAsync(string userId);
        Task<string> RequestResetPasswordByAdminAsync(string userId);
        Task ResetPAsswordAsync(ResetPasswordDTO resetPasswordDTO);

        Task<List<AuthResponseDTO>> GetAllUsersAsync();
        Task<UserDTO> GetUserByIdAsync(string userId);
        Task<bool> DeleteUserAsync(string userId);
    }
}
