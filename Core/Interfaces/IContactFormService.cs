using Core.DTOs.ContactFormDTOs;

namespace Core.Interfaces
{
    public interface IContactFormService
    {
        Task<bool> SubmitContactFormAsync(NewContactFormEntryDTO newEntry);
        Task<List<ContactFormEntryDTO>> GetAllEntriesAsync();
        Task<ContactFormEntryDTO> GetEntryByIdAsync(int id);
        Task<bool> DeleteEntryAsync(int id);
    }
}
