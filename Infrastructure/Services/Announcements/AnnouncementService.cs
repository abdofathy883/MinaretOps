using AutoMapper;
using Core.DTOs.Announcements;
using Core.Enums;
using Core.Interfaces;
using Core.Models;
using Infrastructure.Data;
using Infrastructure.Exceptions;
using Infrastructure.Services.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services.Announcements
{
    public class AnnouncementService : IAnnouncementService
    {
        private readonly MinaretOpsDbContext context;
        private readonly TaskHelperService helperService;
        private readonly IMapper mapper;
        public AnnouncementService(
            MinaretOpsDbContext minaret,
            TaskHelperService _helperService,
            IMapper _mapper
            )
        {
            context = minaret;
            helperService = _helperService;
            mapper = _mapper;
        }
        public async Task<List<AnnouncementDTO>> GetAllAnnouncementsAsync()
        {
            var announcements = await context.Announcements
                .Include(a => a.AnnouncementLinks)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();

            return mapper.Map<List<AnnouncementDTO>>(announcements);
        }
        public async Task<AnnouncementDTO> GetById(int id)
        {
            var announcement = await context.Announcements
                .Include(a => a.AnnouncementLinks)
                .FirstOrDefaultAsync(a => a.Id == id)
                ?? throw new KeyNotFoundException("لا يوجد اعلان لهذا المعرف");
            return mapper.Map<AnnouncementDTO>(announcement);
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
                if (dto.AnnouncementLinks.Any())
                {
                    foreach (var link in dto.AnnouncementLinks)
                    {
                        var announcementLink = new AnnouncementLink
                        {
                            Link = link.Link,
                            Announcement = announcement,
                        };
                        await context.AnnouncementLinks.AddAsync(announcementLink);
                    }
                }
                var employees = await context.Users.ToListAsync();

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

        public async Task<bool> DeleteAsync(int id)
        {
            var announcement = await context.Announcements
                .FirstOrDefaultAsync(a => a.Id == id)
                ?? throw new KeyNotFoundException("لا يوجد اعلان لهذا المعرف");
            context.Announcements.Remove(announcement);
            await context.SaveChangesAsync();
            return true;
        }
    }
}