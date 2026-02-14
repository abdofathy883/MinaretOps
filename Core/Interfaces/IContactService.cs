using Core.DTOs.ContactForm;
using Core.Models;

namespace Core.Interfaces
{
    public interface IContactService
    {
        Task<bool> VerifyTokenAsync(string token);
        Task<bool> CreateContactEntry(NewEntryDTO newEntry);
        Task<List<ContactEntry>> GetEntriesAsync();
        Task<ContactEntry> GetByIdAsync(int id);
    }
}
