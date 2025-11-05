using Core.DTOs.EmployeeOnBoarding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IInvitationService
    {
        Task<InvitationDTO> CreateInvitationAsync(CreateInvitationDTO dto, string adminUserId);
        Task<InvitationDTO> GetInvitationByTokenAsync(string token);
        Task<List<InvitationDTO>> GetPendingInvitationsAsync();
        Task<InvitationDTO> CompleteInvitationAsync(CompleteInvitationDTO dto);
        Task<bool> ApproveInvitationAsync(int invitationId, string adminUserId);
        Task<bool> CancelInvitationAsync(int invitationId);
    }
}
