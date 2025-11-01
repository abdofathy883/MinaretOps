using AutoMapper;
using Core.DTOs.ContactFormDTOs;
using Core.Interfaces;
using Core.Models;
using Infrastructure.Data;
using Infrastructure.Services.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services.ContactForm
{
    public class ContactFormService : IContactFormService
    {
        private readonly MinaretOpsDbContext context;
        private readonly TaskHelperService helperService;
        private readonly IMapper mapper;
        public ContactFormService(
            MinaretOpsDbContext dbContext,
            TaskHelperService _helperService,
            IMapper _mapper
            )
        {
            context = dbContext;
            helperService = _helperService;
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

            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
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
                if (!string.IsNullOrEmpty(entry.Email))
                {
                    var emailPayload = new
                    {
                        To = entry.Email,
                        Subject = "We Got Your Message",
                        Template = "ContactFormSubmission",
                        Replacements = new Dictionary<string, string>
                        {
                            { "{{FullName}}", entry.FullName },
                            { "{{CompanyName}}", entry.CompanyName ?? "Company Name didn't provided" },
                            { "{{Email}}", entry.Email },
                            { "{{PhoneNumber}}", entry.PhoneNumber },
                            { "{{DesiredService}}", entry.DesiredService ?? "You didn't choose specific service"},
                            { "{{ProjectBrief}}", entry.ProjectBrief ?? "You didn't mention your project, we can talk more in a meeting"},
                            { "{{TimeStamp}}", entry.TimeStamp.ToString("f") }
                        }
                    };
                    await helperService.AddOutboxAsync(Core.Enums.OutboxTypes.Email, "New Contact Form Email", emailPayload);
                }
                await context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch(Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
