using Application.DTOs.EmployeeOnBoarding;

namespace Application.Interfaces.Auth
{
    public interface IInvitationService
    {
        Task<InvitationDTO> CreateInvitationAsync(CreateInvitationDTO dto, string adminUserId);
        Task<InvitationDTO> GetInvitationByTokenAsync(string token);
        Task<List<InvitationDTO>> GetPendingInvitationsAsync();
        Task<List<InvitationDTO>> GetAllInvitations();
        Task<InvitationDTO> CompleteInvitationAsync(CompleteInvitationDTO dto);
        Task<bool> ApproveInvitationAsync(int invitationId, string adminUserId);
        Task<bool> CancelInvitationAsync(int invitationId);
    }
}
