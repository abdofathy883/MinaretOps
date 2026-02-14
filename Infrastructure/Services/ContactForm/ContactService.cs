using Core.DTOs.ContactForm;
using Core.Interfaces;
using Core.Models;
using Core.Settings;
using Infrastructure.Data;
using Infrastructure.Services.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace Infrastructure.Services.ContactForm
{
    public class ContactService : IContactService
    {
        private readonly MinaretOpsDbContext context;
        private readonly ILeadService leadService;
        private readonly TaskHelperService helperService;
        private readonly HttpClient httpClient;
        private readonly IOptions<RecaptchaSeetings> options;

        public ContactService(MinaretOpsDbContext context, ILeadService leadService, TaskHelperService taskHelperService, HttpClient httpClient, IOptions<RecaptchaSeetings> options)
        {
            this.context = context;
            this.leadService = leadService;
            this.helperService = taskHelperService;
            this.httpClient = httpClient;
            this.options = options;
        }

        public async Task<bool> CreateContactEntry(NewEntryDTO newEntry)
        {
            var entry = new ContactEntry
            {
                FullName = newEntry.FullName,
                Email = newEntry.Email,
                PhoneNumber = newEntry.PhoneNumber,
                Message = newEntry.Message
            };

            await context.ContactEntries.AddAsync(entry);

            if (!string.IsNullOrEmpty(newEntry.Email))
            {
                var emailPayload = new
                {
                    To = newEntry.Email,
                    Subject = "Task Updates",
                    Template = "ContactForm",
                    Replacements = new Dictionary<string, string>
                    {
                        
                    }
                };
                await helperService.AddOutboxAsync(Core.Enums.OutboxTypes.Email, "New Contact Form Entry", emailPayload);
            }

            //var lead = new CreateLeadDTO
            //{
            //    BusinessName = newEntry.FullName,
            //    WhatsAppNumber = newEntry.PhoneNumber,
            //    ContactStatus = Core.Enums.ContactStatus.NoReply,
            //    LeadSource = Core.Enums.LeadSource.Facebook,
            //    Interested = true,
            //    InterestLevel = Core.Enums.InterestLevel.Hot,

            //};
            //await leadService.CreateLeadAsync(lead);
            return await context.SaveChangesAsync() > 0;
        }

        public async Task<ContactEntry> GetByIdAsync(int id)
        {
            var entry = await context.ContactEntries
                .FirstOrDefaultAsync(e => e.Id == id);
            return entry;
        }

        public async Task<List<ContactEntry>> GetEntriesAsync()
        {
            var entries = await context.ContactEntries.ToListAsync();
            return entries;
        }

        public async Task<bool> VerifyTokenAsync(string token)
        {
            var secretKey = options.Value.SecretKey;
            var response = await httpClient.PostAsync(
            $"https://www.google.com/recaptcha/api/siteverify?secret={secretKey}&response={token}",
            null);

            var json = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<RecaptchaResponse>(json);

            return result!.Success && result.Score >= 0.5;
        }
    }
}
