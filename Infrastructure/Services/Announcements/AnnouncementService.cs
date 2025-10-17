using AutoMapper;
using Core.DTOs.Announcements;
using Core.DTOs.Notifications;
using Core.Enums;
using Core.Interfaces;
using Core.Models;
using Infrastructure.Data;
using Infrastructure.Exceptions;
using Infrastructure.Services.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services.Announcements
{
    public class AnnouncementService: IAnnouncementService
    {
        private readonly MinaretOpsDbContext context;
        private readonly TaskHelperService helperService;
        private readonly INotificationService notificationService;
        private readonly IMapper mapper;
        public AnnouncementService(
            MinaretOpsDbContext minaret,
            TaskHelperService _helperService,
            INotificationService notification,
            IMapper _mapper
            )
        {
            context = minaret;
            helperService = _helperService;
            notificationService = notification;
            mapper = _mapper;
        }

        public async Task<AnnouncementDTO> CreateAnnouncementAsync(CreateAnnouncementDTO dto)
        {
            var existingAnnouncement = await context.Announcements
                .FirstOrDefaultAsync(a => a.Title == dto.Title);

            if (existingAnnouncement is not null)
                throw new AlreadyExistObjectException("اعلان بهذا العنوان موجود بالفعل");

            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                var announcement = new Announcement
                {
                    Title = dto.Title,
                    Message = dto.Message,
                    CreatedAt = DateTime.UtcNow,
                };
                await context.Announcements.AddAsync(announcement);
                var employees = await context.Users.ToListAsync();
                //foreach (var emp in employees)
                //{
                //    var ea = new EmployeeAnnouncement
                //    {
                //        Announcement = announcement,
                //        Employee = emp,
                //        IsRead = false
                //    };
                //    await context.EmployeeAnnouncements.AddAsync(ea);
                //}
                foreach (var emp in employees)
                {
                    if (!string.IsNullOrEmpty(emp.Email))
                    {
                        var emailPayload = new
                        {
                            To = emp.Email,
                            Subject = "New Announcement",
                            Template = "NewAnnouncement",
                            Replacements = new Dictionary<string, string>
                            {
                                { "AnnouncementTitle", announcement.Title },
                                { "AnnouncementContent", announcement.Message },
                                { "AnnouncementId", $"{announcement.Id}" },
                                { "TimeStamp", announcement.CreatedAt.ToString("f") }
                            }
                        };
                        await helperService.AddOutboxAsync(OutboxTypes.Email, "New Announcement Email", emailPayload);                        
                    }
                    var notification = new CreateNotificationDTO
                    {
                        Title = $"New Announcement - {dto.Title}",
                        Body = dto.Message,
                        UserId = emp.Id,
                        Url = "https://internal.theminaretagency.com/announcements"
                    };
                    await notificationService.CreateAsync(notification);
                }
                await context.SaveChangesAsync();
                await transaction.CommitAsync();
                return mapper.Map<AnnouncementDTO>(announcement);
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        public async Task<List<AnnouncementDTO>> GetAllAnnouncementsAsync()
        {
            var announcements = await context.Announcements
                .Include(a => a.EmployeeAnnouncements)
                .ThenInclude(ea => ea.Employee)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();

            return mapper.Map<List<AnnouncementDTO>>(announcements);
        }
    }
}