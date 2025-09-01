using Core.DTOs.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface INotificationService
    {
        Task<NotificationDTO> CreateAsync(CreateNotificationDTO dto);
        Task<IEnumerable<NotificationDTO>> GetTodayForUserAsync(string userId);
        Task MarkAsReadAsync(int id, string userId);
    }
}
