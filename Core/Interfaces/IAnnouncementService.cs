using Core.DTOs.Announcements;

namespace Core.Interfaces
{
    public interface IAnnouncementService
    {
        Task<List<AnnouncementDTO>> GetAllAnnouncementsAsync();
        Task<AnnouncementDTO> CreateAnnouncementAsync(CreateAnnouncementDTO dto);
        Task<AnnouncementDTO> MarkAsReadAsync(int announcementId);

    }
}
