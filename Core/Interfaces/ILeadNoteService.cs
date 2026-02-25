using Core.DTOs.Leads.Notes;

namespace Core.Interfaces
{
    public interface ILeadNoteService
    {
        Task<LeadNoteDTO> CreateNote(CreateLeadNoteDTO leadNote, string currentUserId);
        Task<List<LeadNoteDTO>> GetAllNotes(int leadId);
    }
}
