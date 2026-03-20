using Application.DTOs.Leads.Notes;

namespace Application.Interfaces.Leads
{
    public interface ILeadNoteService
    {
        Task<LeadNoteDTO> CreateNote(CreateLeadNoteDTO leadNote, string currentUserId);
        Task<List<LeadNoteDTO>> GetAllNotes(int leadId);
    }
}
