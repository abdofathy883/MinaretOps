using Core.DTOs.ContactForm;
using Core.DTOs.Leads;
using Core.Interfaces;
using Core.Models;
using Core.Settings;
using Infrastructure.Data;
using Infrastructure.Services.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<ContactService> logger;

        public ContactService(MinaretOpsDbContext context, ILeadService leadService, TaskHelperService taskHelperService, HttpClient httpClient, IOptions<RecaptchaSeetings> options, ILogger<ContactService> logger)
        {
            this.context = context;
            this.leadService = leadService;
            this.helperService = taskHelperService;
            this.httpClient = httpClient;
            this.options = options;
            this.logger = logger;
        }

        public async Task<bool> CreateContactEntry(NewEntryDTO newEntry)
        {
            var entry = new ContactEntry
            {
                FullName = newEntry.FullName,
                Email = newEntry.Email,
                PhoneNumber = newEntry.PhoneNumber,
                Message = newEntry.Message,
                CreatedAt = DateTime.UtcNow
            };

            await context.ContactEntries.AddAsync(entry);

            if (!string.IsNullOrEmpty(newEntry.Email))
            {
                var emailPayload = new
                {
                    To = newEntry.Email,
                    Subject = "We Have Got Your Message",
                    Template = "ContactFormSubmission",
                    Replacements = new Dictionary<string, string>
                    {
                        { "{{FullName}}", newEntry.FullName },
                        { "{{Email}}", newEntry.Email },
                        { "{{PhoneNumber}}", newEntry.PhoneNumber },
                        { "{{Message}}", newEntry.Message ?? string.Empty }
                    }
                };
                await helperService.AddOutboxAsync(Core.Enums.OutboxTypes.Email, "New Contact Form Entry", emailPayload);
            }
            await context.SaveChangesAsync();
            try
            {
                var lead = new CreateLeadDTO
                {
                    BusinessName = newEntry.FullName,
                    WhatsAppNumber = newEntry.PhoneNumber,
                    ContactStatus = Core.Enums.ContactStatus.NotContactedYet,
                    CurrentLeadStatus = Core.Enums.CurrentLeadStatus.NewLead,
                    LeadSource = Core.Enums.LeadSource.Website,
                    Interested = true,
                    InterestLevel = Core.Enums.InterestLevel.Hot,
                };
                await leadService.CreateLeadAsync(lead);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error creating lead from contact form entry with ID {EntryId}", entry.Id);
            }

            return true;
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
            if (string.IsNullOrWhiteSpace(token))
                return false;

            var secretKey = options.Value.SecretKey;
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("secret", secretKey),
                new KeyValuePair<string, string>("response", token)
            });

            var response = await httpClient.PostAsync(
                "https://www.google.com/recaptcha/api/siteverify",
                content);

            if (!response.IsSuccessStatusCode)
                return false;

            var json = await response.Content.ReadAsStringAsync();

            Console.WriteLine(json);

            var result = JsonSerializer.Deserialize<RecaptchaResponse>(json);

            if (result is null)
                return false;

            return result.Success;
        }
    }
}
