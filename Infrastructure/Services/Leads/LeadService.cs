using AutoMapper;
using ClosedXML.Excel;
using Core.DTOs.Leads;
using Core.Enums;
using Core.Interfaces;
using Core.Models;
using Infrastructure.Data;
using Infrastructure.Services.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Reflection;
using System.Security.Claims;
using System.Text.Json;

namespace Infrastructure.Services.Leads
{
    public class LeadService : ILeadService
    {
        private readonly MinaretOpsDbContext context;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly TaskHelperService helperService;
        private readonly IMapper mapper;
        private readonly ILogger<LeadService> logger;
        private readonly IHttpContextAccessor httpContextAccessor;

        public LeadService(MinaretOpsDbContext context, 
            TaskHelperService _helperService, 
            IMapper mapper, 
            UserManager<ApplicationUser> userManager,
            IHttpContextAccessor httpContextAccessor,
            ILogger<LeadService> _logger)
        {
            this.context = context;
            helperService = _helperService;
            this.mapper = mapper;
            this.httpContextAccessor = httpContextAccessor;
            this.userManager = userManager;
            logger = _logger;
        }

        public async Task<LeadDTO> CreateLeadAsync(CreateLeadDTO createLeadDTO, string currentUserId)
        {
            var salesRep = await context.Users.FindAsync(createLeadDTO.SalesRepId)
                ?? throw new KeyNotFoundException($"Sales Representative with ID {createLeadDTO.SalesRepId} not found.");

            var createdBy = await context.Users.FindAsync(currentUserId)
                ?? throw new KeyNotFoundException($"Creator User with ID {createLeadDTO.CreatedById} not found.");

            
            var lead = new SalesLead
            {
                BusinessName = createLeadDTO.BusinessName,
                WhatsAppNumber = createLeadDTO.WhatsAppNumber,
                ContactAttempts = 0,
                ContactStatus = createLeadDTO.ContactStatus,
                LeadSource = createLeadDTO.LeadSource,
                DecisionMakerReached = createLeadDTO.DecisionMakerReached,
                Interested = createLeadDTO.Interested,
                InterestLevel = createLeadDTO.InterestLevel,
                MeetingAgreed = createLeadDTO.MeetingAgreed,
                MeetingDate = createLeadDTO.MeetingDate,
                MeetingAttend = createLeadDTO.MeetingAttend,
                QuotationSent = createLeadDTO.QuotationSent,
                FollowUpReason = createLeadDTO.FollowUpReason,
                FollowUpTime = createLeadDTO.FollowUpTime,
                Notes = createLeadDTO.Notes,
                SalesRepId = salesRep.Id,
                CreatedById = createdBy.Id,
                CreatedAt = DateTime.UtcNow
            };

            context.SalesLeads.Add(lead);

            if (createLeadDTO.ServicesInterestedIn.Any())
            {
                foreach (var ls in createLeadDTO.ServicesInterestedIn)
                {
                    var service = await context.Services.FindAsync(ls.ServiceId);
                    var leadService = new LeadServices
                    {
                        Lead = lead,
                        Service = service
                    };
                    await context.LeadServices.AddAsync(leadService);
                }
            }

            if (!string.IsNullOrEmpty(salesRep.Email))
            {
                var emailPayload = new
                {
                    To = salesRep.Email,
                    Subject = "New Sales Lead Assigned",
                    Template = "",
                    Replacements = new Dictionary<string, string>
                    {

                    }
                };
                await helperService.AddOutboxAsync(OutboxTypes.Email, "New Sales Lead Assigned", emailPayload);
            }

            await context.SaveChangesAsync();

            var mappedLead = await context.SalesLeads
                .Include(x => x.SalesRep)
                .Include(x => x.CreatedBy)
                .Include(x => x.ServicesInterestedIn)
                    .ThenInclude(ls => ls.Service)
                .FirstOrDefaultAsync(x => x.Id == lead.Id);

            return mapper.Map<LeadDTO>(mappedLead);
        }
        public async Task<bool> DeleteLeadAsync(int leadId)
        {
            var lead = await context.SalesLeads
                .FirstOrDefaultAsync(l => l.Id == leadId)
                ?? throw new KeyNotFoundException();
            context.SalesLeads.Remove(lead);
            return await context.SaveChangesAsync() > 0;
        }
        public async Task<List<LeadDTO>> GetAllLeadsAsync(string currentUserId)
        {
            var user = await context.Users.FindAsync(currentUserId);
            if (user is null)
            {
                logger.LogError($"Couldn't find current logged in user with Id: {currentUserId}");
                throw new KeyNotFoundException($"Couldn't find current logged in user with Id: {currentUserId}");
            }

            var roles = httpContextAccessor?.HttpContext?.User?.FindAll(ClaimTypes.Role)
                ?.Select(c => c.Value)
                ?.ToList() ?? new List<string>();

            //var leads = new List<LeadDTO>();
            try
            {
                IQueryable<SalesLead> leadsQuery = context.SalesLeads
                    .AsNoTracking()
                    .Include(x => x.SalesRep)
                    .Include(x => x.CreatedBy)
                    .OrderByDescending(x => x.CreatedAt);

                if (!roles.Contains(UserRoles.Admin.ToString()))
                {
                    leadsQuery = leadsQuery.Where(x => x.SalesRepId == currentUserId);
                }

                var leads = await leadsQuery
                    .Select(x => new LeadDTO
                    {
                        Id = x.Id,
                        BusinessName = x.BusinessName,
                        WhatsAppNumber = x.WhatsAppNumber,
                        ContactAttempts = x.ContactAttempts,
                        ContactStatus = x.ContactStatus,
                        LeadSource = x.LeadSource,
                        DecisionMakerReached = x.DecisionMakerReached,
                        Interested = x.Interested,
                        InterestLevel = x.InterestLevel,
                        MeetingAgreed = x.MeetingAgreed,
                        MeetingDate = x.MeetingDate,
                        MeetingAttend = x.MeetingAttend,
                        QuotationSent = x.QuotationSent,
                        FollowUpTime = x.FollowUpTime,
                        FollowUpReason = x.FollowUpReason,
                        Notes = x.Notes,
                        SalesRepId = x.SalesRepId,
                        SalesRepName = x.SalesRep != null
                        ? x.SalesRep.FirstName + " " + x.SalesRep.LastName
                        : string.Empty,
                        CreatedById = x.CreatedById,
                        CreatedByName = x.CreatedBy.FirstName + " " + x.CreatedBy.LastName,
                        CreatedAt = x.CreatedAt,
                        UpdatedAt = x.UpdatedAt
                    })
                    .ToListAsync();

                return leads;
            }
            catch
            {
                logger.LogError("Get All Leads Method Terminated With Error");
                throw;
            }
        }
        public async Task<LeadDTO> GetLeadByIdAsync(int id)
        {
             var lead = await context.SalesLeads
                .Include(x => x.SalesRep)
                .Include(x => x.CreatedBy)
                .Include(x => x.ServicesInterestedIn)
                    .ThenInclude(ls => ls.Service)
                .FirstOrDefaultAsync(x => x.Id == id)
                ?? throw new KeyNotFoundException($"Lead with ID {id} not found.");

            return mapper.Map<LeadDTO>(lead);
        }
        public async Task<LeadDTO> UpdateLeadAsync(UpdateLeadDTO updateLeadDTO)
        {
             var lead = await context.SalesLeads
                .Include(x => x.ServicesInterestedIn)
                .Include(x => x.SalesRep) // Include SalesRep for email if needed
                .FirstOrDefaultAsync(x => x.Id == updateLeadDTO.Id)
                ?? throw new KeyNotFoundException($"Lead with ID {updateLeadDTO.Id} not found.");

            // Update simple properties
            if (!string.IsNullOrEmpty(updateLeadDTO.BusinessName)) lead.BusinessName = updateLeadDTO.BusinessName;
            if (!string.IsNullOrEmpty(updateLeadDTO.WhatsAppNumber)) lead.WhatsAppNumber = updateLeadDTO.WhatsAppNumber;
            
            lead.ContactAttempts = updateLeadDTO.ContactAttempts;
            lead.ContactStatus = updateLeadDTO.ContactStatus;
            lead.LeadSource = updateLeadDTO.LeadSource;
            lead.DecisionMakerReached = updateLeadDTO.DecisionMakerReached;
            lead.Interested = updateLeadDTO.Interested;
            lead.InterestLevel = updateLeadDTO.InterestLevel;
            lead.MeetingAgreed = updateLeadDTO.MeetingAgreed;
            lead.MeetingDate = updateLeadDTO.MeetingDate;
            lead.MeetingAttend = updateLeadDTO.MeetingAttend;
            lead.QuotationSent = updateLeadDTO.QuotationSent;
            lead.FollowUpTime = updateLeadDTO.FollowUpTime;
            lead.FollowUpReason = updateLeadDTO.FollowUpReason;
            if (updateLeadDTO.Notes != null) lead.Notes = updateLeadDTO.Notes;

            // Handle SalesRep change
            if (!string.IsNullOrEmpty(updateLeadDTO.SalesRepId) && updateLeadDTO.SalesRepId != lead.SalesRepId)
            {
                var newRep = await context.Users.FindAsync(updateLeadDTO.SalesRepId) 
                             ?? throw new KeyNotFoundException($"Sales Rep with ID {updateLeadDTO.SalesRepId} not found.");
                lead.SalesRepId = updateLeadDTO.SalesRepId;
            }

            // Sync Services
            if (updateLeadDTO.ServicesInterestedIn != null)
            {
                var newServiceIds = updateLeadDTO.ServicesInterestedIn; // Already List<int>
                var existingServiceIds = lead.ServicesInterestedIn.Select(x => x.ServiceId).ToList();

                var toRemove = lead.ServicesInterestedIn.Where(x => !newServiceIds.Contains(x.ServiceId)).ToList();
                foreach (var item in toRemove)
                {
                    context.LeadServices.Remove(item); 
                }

                var toAddIds = newServiceIds.Except(existingServiceIds).ToList();
                foreach (var serviceId in toAddIds)
                {
                    lead.ServicesInterestedIn.Add(new LeadServices { LeadId = lead.Id, ServiceId = serviceId });
                }
            }

            lead.UpdatedAt = DateTime.UtcNow;
            context.SalesLeads.Update(lead);

            // Send Email to the assigned employee (SalesRep)
            // Re-fetching or using the loaded/updated SalesRepId
            var assignedRep = await context.Users.FindAsync(lead.SalesRepId);

            if (assignedRep != null && !string.IsNullOrEmpty(assignedRep.Email))
            {
                var emailPayload = new
                {
                    To = assignedRep.Email,
                    Subject = "Lead Updated",
                    Template = "", // Assuming dynamic content or generic template
                    Replacements = new Dictionary<string, string>
                    {
                        { "BusinessName", lead.BusinessName },
                        { "Status", lead.ContactStatus.ToString() }
                    }
                };
                
                // Using TaskHelperService as seen in CreateLeadAsync
                await helperService.AddOutboxAsync(OutboxTypes.Email, "Lead Update Notification", emailPayload);
            }
            await context.SaveChangesAsync();
            // Return updated DTO
            // We use GetLeadByIdAsync to ensure clean mapping with all includes
            return await GetLeadByIdAsync(lead.Id);
        }
        public async Task<LeadDTO> UpdateLeadFieldAsync(int id, string fieldName, object value)
        {
            var lead = await context.SalesLeads
                .Include(x => x.ServicesInterestedIn)
                .FirstOrDefaultAsync(x => x.Id == id)
                ?? throw new KeyNotFoundException($"Lead with ID {id} not found.");

            if (string.Equals(fieldName, "ServicesInterestedIn", StringComparison.OrdinalIgnoreCase))
            {
                // Special handling for Many-to-Many List
                await UpdateLeadServices(lead, value);
            }
            else
            {
                var property = typeof(SalesLead).GetProperty(fieldName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (property == null) throw new ArgumentException($"Property '{fieldName}' not found on SalesLead.");

                // Prevent updating restricted fields
                var restrictedFields = new[] { "Id", "CreatedAt", "CreatedById", "CreatedBy", "ServicesInterestedIn" }; // Services handled above
                if (restrictedFields.Contains(property.Name)) throw new ArgumentException($"Updating property '{fieldName}' is not allowed or handled separately.");

                try
                {
                    object? convertedValue = null;
                    if (value != null)
                    {
                        Type targetType = property.PropertyType;
                        if (Nullable.GetUnderlyingType(targetType) != null)
                        {
                            targetType = Nullable.GetUnderlyingType(targetType)!;
                        }

                        if (value is JsonElement jsonElement)
                        {
                             convertedValue = JsonSerializer.Deserialize(jsonElement.GetRawText(), property.PropertyType);
                        }
                        else
                        {
                            convertedValue = Convert.ChangeType(value, targetType);
                        }
                    }

                    property.SetValue(lead, convertedValue);
                }
                 catch (Exception ex)
                {
                   throw new ArgumentException($"Invalid value for property '{fieldName}': {ex.Message}");
                }
            }

            lead.UpdatedAt = DateTime.UtcNow;
            await context.SaveChangesAsync();
            
            return await GetLeadByIdAsync(id);
        }
        private async Task UpdateLeadServices(SalesLead lead, object value)
        {
            List<int> newServiceIds = new();

            if (value is JsonElement jsonElement && jsonElement.ValueKind == JsonValueKind.Array)
            {
                 newServiceIds = JsonSerializer.Deserialize<List<int>>(jsonElement.GetRawText()) ?? new();
            }
            // If passed as List<LeadServices> or similar from internal calls, handle here if needed.
            
            // Sync Services: Remove ones not in new list, Add new ones
            var existingServiceIds = lead.ServicesInterestedIn.Select(x => x.ServiceId).ToList();
            
            // To Remove
            var toRemove = lead.ServicesInterestedIn.Where(x => !newServiceIds.Contains(x.ServiceId)).ToList();
            foreach(var item in toRemove) {
                context.Set<LeadServices>().Remove(item); // Or lead.ServicesInterestedIn.Remove(item) if cascading is set right, but explicit is safer
            }

            // To Add
            var toAddIds = newServiceIds.Except(existingServiceIds).ToList();
            foreach (var serviceId in toAddIds)
            {
                lead.ServicesInterestedIn.Add(new LeadServices { LeadId = lead.Id, ServiceId = serviceId });
            }
        }
        public async Task ImportLeadsFromExcelAsync(Stream fileStream, string currentUserId)
        {
            using var workbook = new XLWorkbook(fileStream);
            var worksheet = workbook.Worksheet(1);
            var rows = worksheet.RangeUsed().RowsUsed().Skip(1); // Skip header

            foreach (var row in rows)
            {
                var whatsapp = row.Cell(2).GetValue<string>(); // Assume Col 2 is WhatsApp
                if (string.IsNullOrWhiteSpace(whatsapp)) continue;

                var existingLead = await context.SalesLeads
                    .Include(l => l.ServicesInterestedIn)
                    .FirstOrDefaultAsync(l => l.WhatsAppNumber == whatsapp);

                if (existingLead != null)
                {
                    // Update
                    existingLead.BusinessName = row.Cell(1).GetValue<string>();
                    existingLead.ContactStatus = ParseEnum<ContactStatus>(row.Cell(3).GetValue<string>());
                    existingLead.ContactAttempts = row.Cell(4).GetValue<int>();
                    existingLead.LeadSource = ParseEnum<LeadSource>(row.Cell(5).GetValue<string>());
                    existingLead.DecisionMakerReached = row.Cell(6).GetValue<bool>();
                    existingLead.Interested = row.Cell(7).GetValue<bool>();
                    existingLead.InterestLevel = ParseEnum<InterestLevel>(row.Cell(8).GetValue<string>());
                    existingLead.MeetingAgreed = row.Cell(9).GetValue<bool>();
                    existingLead.FollowUpReason = ParseEnum<FollowUpReason>(row.Cell(14).GetValue<string>());
                    existingLead.Notes = row.Cell(15).GetValue<string>();

                    existingLead.UpdatedAt = DateTime.UtcNow;
                    // Note: Skipping Services, Meeting Date, etc for brevity/complexity in first pass unless requested

                    context.SalesLeads.Update(existingLead);
                }
                else
                {
                    // Create
                    var newLead = new SalesLead
                    {
                        BusinessName = row.Cell(1).GetValue<string>(),
                        WhatsAppNumber = whatsapp,
                        ContactStatus = ParseEnum<ContactStatus>(row.Cell(3).GetValue<string>()),
                        ContactAttempts = row.Cell(4).GetValue<int>(),
                        LeadSource = ParseEnum<LeadSource>(row.Cell(5).GetValue<string>()),
                        DecisionMakerReached = row.Cell(6).GetValue<bool>(),
                        Interested = row.Cell(7).GetValue<bool>(),
                        InterestLevel = ParseEnum<InterestLevel>(row.Cell(8).GetValue<string>()),
                        MeetingAgreed = row.Cell(9).GetValue<bool>(),
                        FollowUpReason = ParseEnum<FollowUpReason>(row.Cell(14).GetValue<string>()),
                        Notes = row.Cell(15).GetValue<string>(),
                        CreatedById = currentUserId,
                        SalesRepId = currentUserId,
                        CreatedAt = DateTime.UtcNow
                    };
                    // Handle default/optional fields
                    context.SalesLeads.Add(newLead);
                }
            }
            await context.SaveChangesAsync();
        }
        private T ParseEnum<T>(string value) where T : struct
        {
            if (string.IsNullOrWhiteSpace(value)) return default;
            return Enum.TryParse<T>(value, true, out var result) ? result : default;
        }
        public async Task<byte[]> ExportLeadsToExcelAsync(string userId)
        {
            var leads = await GetAllLeadsAsync(userId);
            // Although GetAllLeadsAsync returns DTOs, we might want entities or just map DTOs.
            // DTOs are fine.

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Leads");

            // Headers
            worksheet.Cell(1, 1).Value = "Business Name";
            worksheet.Cell(1, 2).Value = "WhatsApp Number";
            worksheet.Cell(1, 3).Value = "Contact Status";
            worksheet.Cell(1, 4).Value = "Contact Attempts";
            worksheet.Cell(1, 5).Value = "Lead Source";
            worksheet.Cell(1, 6).Value = "Decision Maker Reached";
            worksheet.Cell(1, 7).Value = "Interested";
            worksheet.Cell(1, 8).Value = "Interest Level";
            worksheet.Cell(1, 9).Value = "Meeting Agreed";
            worksheet.Cell(1, 10).Value = "Meeting Date";
            worksheet.Cell(1, 11).Value = "Meeting Attend";
            worksheet.Cell(1, 12).Value = "Quotation Sent";
            worksheet.Cell(1, 13).Value = "Follow Up Time";
            worksheet.Cell(1, 14).Value = "Follow Up Reason";
            worksheet.Cell(1, 15).Value = "Notes";

            int row = 2;
            foreach (var lead in leads)
            {
                worksheet.Cell(row, 1).Value = lead.BusinessName;
                worksheet.Cell(row, 2).Value = lead.WhatsAppNumber;
                worksheet.Cell(row, 3).Value = lead.ContactStatus.ToString();
                worksheet.Cell(row, 4).Value = lead.ContactAttempts;
                worksheet.Cell(row, 5).Value = lead.LeadSource.ToString();
                worksheet.Cell(row, 6).Value = lead.DecisionMakerReached;
                worksheet.Cell(row, 7).Value = lead.Interested;
                worksheet.Cell(row, 8).Value = lead.InterestLevel.ToString();
                worksheet.Cell(row, 9).Value = lead.MeetingAgreed;
                worksheet.Cell(row, 10).Value = lead.MeetingDate?.ToString();
                worksheet.Cell(row, 11).Value = lead.MeetingAttend.ToString();
                worksheet.Cell(row, 12).Value = lead.QuotationSent;
                worksheet.Cell(row, 13).Value = lead.FollowUpTime?.ToString();
                worksheet.Cell(row, 14).Value = lead.FollowUpReason.ToString();
                worksheet.Cell(row, 15).Value = lead.Notes;
                row++;
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }
        public async Task<byte[]> GetLeadTemplateAsync()
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Leads Template");

            // Headers
            worksheet.Cell(1, 1).Value = "Client Name";
            worksheet.Cell(1, 2).Value = "WhatsApp Number";
            worksheet.Cell(1, 3).Value = "Contact Status";
            worksheet.Cell(1, 4).Value = "Contact Attempts";
            worksheet.Cell(1, 5).Value = "Lead Source";
            worksheet.Cell(1, 6).Value = "Decision Maker Reached";
            worksheet.Cell(1, 7).Value = "Interested";
            worksheet.Cell(1, 8).Value = "Interest Level";
            worksheet.Cell(1, 9).Value = "Meeting Agreed";
            worksheet.Cell(1, 10).Value = "Meeting Date";
            worksheet.Cell(1, 11).Value = "Meeting Attend";
            worksheet.Cell(1, 12).Value = "Quotation Sent";
            worksheet.Cell(1, 13).Value = "Follow Up Time";
            worksheet.Cell(1, 14).Value = "Follow Up Reason";
            worksheet.Cell(1, 15).Value = "Notes";


            // Sample row with dropdown and tick-box friendly values
            worksheet.Cell(2, 1).Value = "Mohamed Ahmed";
            worksheet.Cell(2, 2).Value = "01012345678";
            worksheet.Cell(2, 3).Value = "";
            worksheet.Cell(2, 4).Value = 0;
            worksheet.Cell(2, 5).Value = LeadSource.Facebook.ToString();
            worksheet.Cell(2, 6).Value = false;
            worksheet.Cell(2, 7).Value = false;
            worksheet.Cell(2, 8).Value = InterestLevel.Cold.ToString();
            worksheet.Cell(2, 9).Value = false;
            worksheet.Cell(2, 10).Value = null as string;
            worksheet.Cell(2, 11).Value = MeetingAttend.Pending.ToString();
            worksheet.Cell(2, 12).Value = false;
            worksheet.Cell(2, 13).Value = null as string;
            worksheet.Cell(2, 14).Value = FollowUpReason.Later.ToString();
            worksheet.Cell(2, 15).Value = "Sample notes";

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

    }
}
