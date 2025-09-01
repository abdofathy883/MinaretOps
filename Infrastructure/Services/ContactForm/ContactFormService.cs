using AutoMapper;
using Core.DTOs.ContactFormDTOs;
using Core.Interfaces;
using Core.Models;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services.ContactForm
{
    public class ContactFormService : IContactFormService
    {
        private readonly MinaretOpsDbContext context;
        private readonly IEmailService emailService;
        private readonly IMapper mapper;
        public ContactFormService(
            MinaretOpsDbContext dbContext,
            IEmailService email,
            IMapper _mapper
            )
        {
            context = dbContext;
            emailService = email;
            mapper = _mapper;
        }
        public async Task<bool> DeleteEntryAsync(int id)
        {
            var entry = await context.ContactFormEntries.FirstOrDefaultAsync(e => e.Id == id)
                ?? throw new Exception("Entry not found");

            context.ContactFormEntries.Remove(entry);
            return await context.SaveChangesAsync() > 0;
        }

        public async Task<List<ContactFormEntryDTO>> GetAllEntriesAsync()
        {
            var entries = await context.ContactFormEntries.ToListAsync()
                ?? throw new Exception("No entries found");

            return mapper.Map<List<ContactFormEntryDTO>>(entries);
        }

        public async Task<ContactFormEntryDTO> GetEntryByIdAsync(int id)
        {
            var entry = await context.ContactFormEntries
                .FirstOrDefaultAsync(e => e.Id == id)
                ?? throw new Exception("Entry not found");

            return mapper.Map<ContactFormEntryDTO>(entry);
        }

        public async Task<bool> SubmitContactFormAsync(NewContactFormEntryDTO newEntry)
        {
            if(newEntry is null)
                throw new Exception("Invalid entry");

            ContactFormEntry entry = new ContactFormEntry
            {
                FullName = newEntry.FullName,
                CompanyName = newEntry.CompanyName,
                Email = newEntry.Email,
                PhoneNumber = newEntry.PhoneNumber,
                DesiredService = newEntry.DesiredService,
                ProjectBrief = newEntry.ProjectBrief,
                TimeStamp = DateTime.UtcNow
            };

            await context.ContactFormEntries.AddAsync(entry);
            await context.SaveChangesAsync();

            Dictionary<string, string> placeholders = new Dictionary<string, string>
            {
                { "{{FullName}}", entry.FullName },
                { "{{CompanyName}}", entry.CompanyName },
                { "{{Email}}", entry.Email },
                { "{{PhoneNumber}}", entry.PhoneNumber },
                { "{{DesiredService}}", entry.DesiredService },
                { "{{ProjectBrief}}", entry.ProjectBrief },
                { "{{TimeStamp}}", entry.TimeStamp.ToString("f") }
            };

            await emailService.SendEmailWithTemplateAsync(newEntry.Email, "New Submission", "contact-form-submission", placeholders);

            return true;
        }
    }
}
