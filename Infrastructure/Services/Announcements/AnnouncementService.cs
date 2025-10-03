using AutoMapper;
using Core.DTOs.Announcements;
using Core.DTOs.Notifications;
using Core.Interfaces;
using Core.Models;
using Infrastructure.Data;
using Infrastructure.Exceptions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services.Announcements
{
    public class AnnouncementService: IAnnouncementService
    {
        private readonly MinaretOpsDbContext context;
        private readonly IEmailService emailService;
        private readonly INotificationService notificationService;
        private readonly IMapper mapper;
        public AnnouncementService(
            MinaretOpsDbContext minaret,
            IEmailService email,
            INotificationService notification,
            IMapper _mapper
            )
        {
            context = minaret;
            emailService = email;
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
                foreach (var emp in employees)
                {
                    var ea = new EmployeeAnnouncement
                    {
                        Announcement = announcement,
                        Employee = emp,
                        IsRead = false
                    };
                    await context.EmployeeAnnouncements.AddAsync(ea);
                }
                await context.SaveChangesAsync();
                Dictionary<string, string> replacements = new Dictionary<string, string>
                {
                    { "AnnouncementTitle", announcement.Title },
                    { "AnnouncementContent", announcement.Message },
                    { "AnnouncementId", $"{announcement.Id}" },
                    { "{{TimeStamp}}", announcement.CreatedAt.ToString("f") }
                };
                foreach (var emp in employees)
                {
                    if (!string.IsNullOrEmpty(emp.Email))
                    {
                        await emailService.SendEmailWithTemplateAsync(emp.Email ?? string.Empty, "New Announcement", "NewAnnouncement", replacements);
                        
                    }
                    //var notification = new CreateNotificationDTO
                    //{
                    //    Title = $"New Announcement - {dto.Title}",
                    //    Body = dto.Message,
                    //    UserId = emp.Id,
                    //    Url = "https://internal.theminaretagency.com/announcements"
                    //};
                    //await notificationService.CreateAsync(notification);
                }
                await transaction.CommitAsync();
                return mapper.Map<AnnouncementDTO>(announcement);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception(ex.Message);
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

        //public Task<AnnouncementDTO> MarkAsReadAsync(int announcementId)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
