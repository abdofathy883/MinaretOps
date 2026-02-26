using AutoMapper;
using ClosedXML.Excel;
using Core.DTOs.Leads;
using Core.DTOs.Leads.Notes;
using Core.Enums;
using Core.Enums.Auth_Attendance;
using Core.Enums.Leads;
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

        private async Task<(string Id, string FullName)?> GetCurrentUserForHistoryAsync()
        {
            var userId = httpContextAccessor?.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return null;
            var user = await context.Users.FindAsync(userId);
            if (user == null) return null;
            return (user.Id, $"{user.FirstName} {user.LastName}");
        }

        private static string ToHistoryString(object? value)
        {
            if (value == null) return string.Empty;
            if (value is DateTime dt) return dt.ToString("u");
            return value.ToString() ?? string.Empty;
        }

        public async Task<LeadDTO> CreateLeadAsync(CreateLeadDTO createLeadDTO, string? currentUserId = null)
        {
            var salesRep = await context.Users.FindAsync(createLeadDTO.SalesRepId);

            var createdBy = await context.Users.FindAsync(currentUserId)
                ?? throw new KeyNotFoundException();

            
            var lead = new SalesLead
            {
                BusinessName = createLeadDTO.BusinessName,
                WhatsAppNumber = createLeadDTO.WhatsAppNumber.Trim(),
                Country = createLeadDTO.Country,
                Occupation = createLeadDTO.Occupation,
                ContactStatus = createLeadDTO.ContactStatus,
                CurrentLeadStatus = createLeadDTO.CurrentLeadStatus,
                LeadSource = createLeadDTO.LeadSource,
                InterestLevel = createLeadDTO.InterestLevel,
                FreelancePlatform = createLeadDTO.FreelancePlatform,
                Responsibility = createLeadDTO.Responsibility,
                Budget = createLeadDTO.Budget,
                Timeline = createLeadDTO.Timeline,
                NeedsExpectation = createLeadDTO.NeedsExpectation,
                MeetingDate = createLeadDTO.MeetingDate,
                FollowUpTime = createLeadDTO.FollowUpTime,
                QuotationSent = createLeadDTO.QuotationSent,
                //SalesRepId = salesRep.Id ?? string.Empty,
                CreatedById = createdBy.Id,
                CreatedAt = DateTime.UtcNow
            };

            if (salesRep is not null)
            {
                lead.SalesRepId = salesRep.Id;
            }

            context.SalesLeads.Add(lead);

            var createdEntry = new SalesLeadHistory
            {
                SalesLead = lead,
                PropertyName = "إنشاء العميل",
                OldValue = null,
                NewValue = lead.BusinessName,
                UpdatedById = createdBy.Id,
                UpdatedByName = $"{createdBy.FirstName} {createdBy.LastName}",
                UpdatedAt = DateTime.UtcNow
            };
            await context.LeadHistory.AddAsync(createdEntry);

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

            if (createLeadDTO.Notes.Any())
            {
                foreach (var note in createLeadDTO.Notes)
                {
                    var newNote = new LeadNote
                    {
                        Note = note.Note,
                        Lead = lead,
                        CreatedById = createdBy.Id,
                        CreatedAt = DateTime.UtcNow
                    };
                    lead.Notes.Add(newNote);
                }
            }

            if (salesRep is not null && !string.IsNullOrEmpty(salesRep.Email))
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
                    .Include(x => x.Notes)
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
                        ContactStatus = x.ContactStatus,
                        CurrentLeadStatus = x.CurrentLeadStatus,
                        InterestLevel = x.InterestLevel,
                        MeetingDate = x.MeetingDate,
                        QuotationSent = x.QuotationSent,
                        FollowUpTime = x.FollowUpTime,
                        SalesRepId = x.SalesRepId,
                        SalesRepName = x.SalesRep != null
                        ? x.SalesRep.FirstName + " " + x.SalesRep.LastName
                        : string.Empty
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
                .AsSingleQuery()
               .Include(x => x.SalesRep)
               .Include(x => x.CreatedBy)
               .Include(x => x.Notes)
               .Include(x => x.LeadHistory)
               .Include(x => x.ServicesInterestedIn)
                   .ThenInclude(ls => ls.Service)
               .SingleAsync(x => x.Id == id);

            return mapper.Map<LeadDTO>(lead);
        }
        public async Task<LeadDTO> UpdateLeadAsync(UpdateLeadDTO updateLeadDTO)
        {
             var lead = await context.SalesLeads
                .Include(x => x.ServicesInterestedIn)
                .Include(x => x.Notes)
                .Include(x => x.SalesRep)
                .SingleAsync(x => x.Id == updateLeadDTO.Id)
                ?? throw new KeyNotFoundException($"Lead with ID {updateLeadDTO.Id} not found.");

            var user = await GetCurrentUserForHistoryAsync();
            var histories = new List<SalesLeadHistory>();
            var now = DateTime.UtcNow;
            string UserId() => user?.Id ?? string.Empty;
            string UserName() => user?.FullName ?? "—";

            // Update simple properties and record history
            if (!string.IsNullOrEmpty(updateLeadDTO.BusinessName) && updateLeadDTO.BusinessName != lead.BusinessName)
            {
                histories.Add(new SalesLeadHistory { SalesLeadId = lead.Id, PropertyName = "اسم العميل", OldValue = lead.BusinessName, NewValue = updateLeadDTO.BusinessName, UpdatedById = UserId(), UpdatedByName = UserName(), UpdatedAt = now });
                lead.BusinessName = updateLeadDTO.BusinessName;
            }
            if (!string.IsNullOrEmpty(updateLeadDTO.WhatsAppNumber) && updateLeadDTO.WhatsAppNumber != lead.WhatsAppNumber)
            {
                histories.Add(new SalesLeadHistory { SalesLeadId = lead.Id, PropertyName = "رقم الواتساب", OldValue = lead.WhatsAppNumber, NewValue = updateLeadDTO.WhatsAppNumber, UpdatedById = UserId(), UpdatedByName = UserName(), UpdatedAt = now });
                lead.WhatsAppNumber = updateLeadDTO.WhatsAppNumber;
            }
            if (updateLeadDTO.Country != lead.Country)
            {
                histories.Add(new SalesLeadHistory { SalesLeadId = lead.Id, PropertyName = "البلد", OldValue = lead.Country, NewValue = updateLeadDTO.Country, UpdatedById = UserId(), UpdatedByName = UserName(), UpdatedAt = now });
                lead.Country = updateLeadDTO.Country;
            }
            if (updateLeadDTO.Occupation != lead.Occupation)
            {
                histories.Add(new SalesLeadHistory { SalesLeadId = lead.Id, PropertyName = "المجال", OldValue = lead.Occupation, NewValue = updateLeadDTO.Occupation, UpdatedById = UserId(), UpdatedByName = UserName(), UpdatedAt = now });
                lead.Occupation = updateLeadDTO.Occupation;
            }
            if (updateLeadDTO.ContactStatus != lead.ContactStatus)
            {
                histories.Add(new SalesLeadHistory { SalesLeadId = lead.Id, PropertyName = "حالة التواصل", OldValue = lead.ContactStatus.ToString(), NewValue = updateLeadDTO.ContactStatus.ToString(), UpdatedById = UserId(), UpdatedByName = UserName(), UpdatedAt = now });
                lead.ContactStatus = updateLeadDTO.ContactStatus;
            }
            if (updateLeadDTO.CurrentLeadStatus != lead.CurrentLeadStatus)
            {
                histories.Add(new SalesLeadHistory { SalesLeadId = lead.Id, PropertyName = "حالة العميل الحالية", OldValue = lead.CurrentLeadStatus.ToString(), NewValue = updateLeadDTO.CurrentLeadStatus.ToString(), UpdatedById = UserId(), UpdatedByName = UserName(), UpdatedAt = now });
                lead.CurrentLeadStatus = updateLeadDTO.CurrentLeadStatus;
            }
            if (updateLeadDTO.LeadSource != lead.LeadSource)
            {
                histories.Add(new SalesLeadHistory { SalesLeadId = lead.Id, PropertyName = "المصدر", OldValue = lead.LeadSource.ToString(), NewValue = updateLeadDTO.LeadSource.ToString(), UpdatedById = UserId(), UpdatedByName = UserName(), UpdatedAt = now });
                lead.LeadSource = updateLeadDTO.LeadSource;
            }
            if (updateLeadDTO.InterestLevel != lead.InterestLevel)
            {
                histories.Add(new SalesLeadHistory { SalesLeadId = lead.Id, PropertyName = "درجة الاهتمام", OldValue = lead.InterestLevel.ToString(), NewValue = updateLeadDTO.InterestLevel.ToString(), UpdatedById = UserId(), UpdatedByName = UserName(), UpdatedAt = now });
                lead.InterestLevel = updateLeadDTO.InterestLevel;
            }
            if (updateLeadDTO.Responsibility != lead.Responsibility)
            {
                histories.Add(new SalesLeadHistory { SalesLeadId = lead.Id, PropertyName = "المسئولية", OldValue = lead.Responsibility.ToString(), NewValue = updateLeadDTO.Responsibility.ToString(), UpdatedById = UserId(), UpdatedByName = UserName(), UpdatedAt = now });
                lead.Responsibility = updateLeadDTO.Responsibility;
            }
            if (updateLeadDTO.Budget != lead.Budget)
            {
                histories.Add(new SalesLeadHistory { SalesLeadId = lead.Id, PropertyName = "الميزانية", OldValue = lead.Budget.ToString(), NewValue = updateLeadDTO.Budget.ToString(), UpdatedById = UserId(), UpdatedByName = UserName(), UpdatedAt = now });
                lead.Budget = updateLeadDTO.Budget;
            }
            if (updateLeadDTO.Timeline != lead.Timeline)
            {
                histories.Add(new SalesLeadHistory { SalesLeadId = lead.Id, PropertyName = "مدة التنفيذ", OldValue = lead.Timeline.ToString(), NewValue = updateLeadDTO.Timeline.ToString(), UpdatedById = UserId(), UpdatedByName = UserName(), UpdatedAt = now });
                lead.Timeline = updateLeadDTO.Timeline;
            }
            if (updateLeadDTO.NeedsExpectation != lead.NeedsExpectation)
            {
                histories.Add(new SalesLeadHistory { SalesLeadId = lead.Id, PropertyName = "المتطلبات", OldValue = lead.NeedsExpectation.ToString(), NewValue = updateLeadDTO.NeedsExpectation.ToString(), UpdatedById = UserId(), UpdatedByName = UserName(), UpdatedAt = now });
                lead.NeedsExpectation = updateLeadDTO.NeedsExpectation;
            }
            if (updateLeadDTO.MeetingDate != lead.MeetingDate)
            {
                histories.Add(new SalesLeadHistory { SalesLeadId = lead.Id, PropertyName = "تاريخ الاجتماع", OldValue = ToHistoryString(lead.MeetingDate), NewValue = ToHistoryString(updateLeadDTO.MeetingDate), UpdatedById = UserId(), UpdatedByName = UserName(), UpdatedAt = now });
                lead.MeetingDate = updateLeadDTO.MeetingDate;
            }
            if (updateLeadDTO.FollowUpTime != lead.FollowUpTime)
            {
                histories.Add(new SalesLeadHistory { SalesLeadId = lead.Id, PropertyName = "تاريخ المتابعة", OldValue = ToHistoryString(lead.FollowUpTime), NewValue = ToHistoryString(updateLeadDTO.FollowUpTime), UpdatedById = UserId(), UpdatedByName = UserName(), UpdatedAt = now });
                lead.FollowUpTime = updateLeadDTO.FollowUpTime;
            }
            if (updateLeadDTO.QuotationSent != lead.QuotationSent)
            {
                histories.Add(new SalesLeadHistory { SalesLeadId = lead.Id, PropertyName = "تم إرسال عرض سعر", OldValue = lead.QuotationSent.ToString(), NewValue = updateLeadDTO.QuotationSent.ToString(), UpdatedById = UserId(), UpdatedByName = UserName(), UpdatedAt = now });
                lead.QuotationSent = updateLeadDTO.QuotationSent;
            }
            if (updateLeadDTO.LeadSource == LeadSource.FreelancingPlatforms && updateLeadDTO.FreelancePlatform != lead.FreelancePlatform)
            {
                histories.Add(new SalesLeadHistory { SalesLeadId = lead.Id, PropertyName = "المنصة", OldValue = lead.FreelancePlatform?.ToString() ?? "", NewValue = updateLeadDTO.FreelancePlatform?.ToString() ?? "", UpdatedById = UserId(), UpdatedByName = UserName(), UpdatedAt = now });
                lead.FreelancePlatform = updateLeadDTO.FreelancePlatform;
            }

            if (!string.IsNullOrEmpty(updateLeadDTO.SalesRepId) && updateLeadDTO.SalesRepId != lead.SalesRepId)
            {
                var newRep = await context.Users.FindAsync(updateLeadDTO.SalesRepId)
                             ?? throw new KeyNotFoundException($"Sales Rep with ID {updateLeadDTO.SalesRepId} not found.");
                var oldRepName = lead.SalesRep != null ? $"{lead.SalesRep.FirstName} {lead.SalesRep.LastName}" : "";
                histories.Add(new SalesLeadHistory { SalesLeadId = lead.Id, PropertyName = "المسئول عن العميل", OldValue = oldRepName, NewValue = $"{newRep.FirstName} {newRep.LastName}", UpdatedById = UserId(), UpdatedByName = UserName(), UpdatedAt = now });
                lead.SalesRepId = updateLeadDTO.SalesRepId;
            }

            if (updateLeadDTO.ServicesInterestedIn != null)
            {
                var newServiceIds = updateLeadDTO.ServicesInterestedIn;
                var existingServiceIds = lead.ServicesInterestedIn.Select(x => x.ServiceId).ToList();
                if (newServiceIds.Count != existingServiceIds.Count || newServiceIds.Except(existingServiceIds).Any() || existingServiceIds.Except(newServiceIds).Any())
                {
                    var oldTitles = string.Join(", ", lead.ServicesInterestedIn.Select(x => x.Service?.Title).Where(t => !string.IsNullOrEmpty(t)));
                    var newTitlesList = await context.Services.Where(s => newServiceIds.Contains(s.Id)).Select(s => s.Title).ToListAsync();
                    var newTitles = string.Join(", ", newTitlesList);
                    histories.Add(new SalesLeadHistory { SalesLeadId = lead.Id, PropertyName = "الخدمات المطلوبة", OldValue = oldTitles, NewValue = newTitles, UpdatedById = UserId(), UpdatedByName = UserName(), UpdatedAt = now });
                }

                var toRemove = lead.ServicesInterestedIn.Where(x => !newServiceIds.Contains(x.ServiceId)).ToList();
                foreach (var item in toRemove)
                    context.LeadServices.Remove(item);
                var toAddIds = newServiceIds.Except(existingServiceIds).ToList();
                foreach (var serviceId in toAddIds)
                    lead.ServicesInterestedIn.Add(new LeadServices { LeadId = lead.Id, ServiceId = serviceId });
            }

            lead.UpdatedAt = now;
            context.SalesLeads.Update(lead);
            if (histories.Any())
                await context.LeadHistory.AddRangeAsync(histories);

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
        private static readonly Dictionary<string, string> LeadPropertyDisplayNames = new(StringComparer.OrdinalIgnoreCase)
        {
            ["BusinessName"] = "اسم العميل", ["WhatsAppNumber"] = "رقم الواتساب", ["Country"] = "البلد", ["Occupation"] = "المجال",
            ["ContactStatus"] = "حالة التواصل", ["CurrentLeadStatus"] = "حالة العميل الحالية", ["LeadSource"] = "المصدر", ["InterestLevel"] = "درجة الاهتمام",
            ["FreelancePlatform"] = "المنصة", ["Responsibility"] = "المسئولية", ["Budget"] = "الميزانية", ["Timeline"] = "مدة التنفيذ",
            ["NeedsExpectation"] = "المتطلبات", ["MeetingDate"] = "تاريخ الاجتماع", ["FollowUpTime"] = "تاريخ المتابعة", ["QuotationSent"] = "تم إرسال عرض سعر",
            ["SalesRepId"] = "المسئول عن العميل", ["ServicesInterestedIn"] = "الخدمات المطلوبة"
        };

        public async Task<LeadDTO> UpdateLeadFieldAsync(int id, string fieldName, object value)
        {
            var lead = await context.SalesLeads
                .Include(x => x.ServicesInterestedIn)
                    .ThenInclude(ls => ls.Service)
                .Include(x => x.SalesRep)
                .FirstOrDefaultAsync(x => x.Id == id)
                ?? throw new KeyNotFoundException($"Lead with ID {id} not found.");

            var user = await GetCurrentUserForHistoryAsync();
            string UserId() => user?.Id ?? string.Empty;
            string UserName() => user?.FullName ?? "—";
            var now = DateTime.UtcNow;
            string? oldValueStr = null;
            string? newValueStr = null;

            if (string.Equals(fieldName, "ServicesInterestedIn", StringComparison.OrdinalIgnoreCase))
            {
                oldValueStr = string.Join(", ", lead.ServicesInterestedIn.Select(x => x.Service?.Title).Where(t => !string.IsNullOrEmpty(t)));
                await UpdateLeadServices(lead, value);
                var newIds = value is JsonElement je && je.ValueKind == JsonValueKind.Array
                    ? JsonSerializer.Deserialize<List<int>>(je.GetRawText()) ?? new List<int>()
                    : new List<int>();
                var newTitles = await context.Services.Where(s => newIds.Contains(s.Id)).Select(s => s.Title).ToListAsync();
                newValueStr = string.Join(", ", newTitles);
            }
            else
            {
                var property = typeof(SalesLead).GetProperty(fieldName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (property == null) throw new ArgumentException($"Property '{fieldName}' not found on SalesLead.");

                var restrictedFields = new[] { "Id", "CreatedAt", "CreatedById", "CreatedBy", "ServicesInterestedIn", "LeadHistory", "Notes" };
                if (restrictedFields.Contains(property.Name)) throw new ArgumentException($"Updating property '{fieldName}' is not allowed or handled separately.");

                oldValueStr = ToHistoryString(property.GetValue(lead));
                try
                {
                    object? convertedValue = null;
                    if (value != null)
                    {
                        Type targetType = property.PropertyType;
                        if (Nullable.GetUnderlyingType(targetType) != null)
                            targetType = Nullable.GetUnderlyingType(targetType)!;
                        if (value is JsonElement jsonElement)
                            convertedValue = JsonSerializer.Deserialize(jsonElement.GetRawText(), property.PropertyType);
                        else
                            convertedValue = Convert.ChangeType(value, targetType);
                    }
                    property.SetValue(lead, convertedValue);
                    newValueStr = ToHistoryString(convertedValue);
                }
                catch (Exception ex)
                {
                    throw new ArgumentException($"Invalid value for property '{fieldName}': {ex.Message}");
                }
            }

            var propDisplay = LeadPropertyDisplayNames.TryGetValue(fieldName, out var name) ? name : fieldName;
            var historyEntry = new SalesLeadHistory
            {
                SalesLeadId = lead.Id,
                PropertyName = propDisplay,
                OldValue = oldValueStr,
                NewValue = newValueStr,
                UpdatedById = UserId(),
                UpdatedByName = UserName(),
                UpdatedAt = now
            };
            await context.LeadHistory.AddAsync(historyEntry);
            lead.UpdatedAt = now;
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
        public async Task<List<LeadDTO>> SearchLeadsAsync(string query, string currentUserId)
        {
            if (string.IsNullOrWhiteSpace(query))
                return new List<LeadDTO>();

            query = query.Trim();
            //var isId = int.TryParse(query, out int taskId);

            var emp = await helperService.GetUserOrThrow(currentUserId);

            var roles = await userManager.GetRolesAsync(emp);

            IQueryable<SalesLead> leadsQuery = context.SalesLeads
                .Include(t => t.ServicesInterestedIn)
                .Include(t => t.SalesRep)
                .Include(t => t.CreatedBy);

            // Role-based filtering
            if (roles.Contains("Admin"))
            {
                // no extra filter
            }
            else
            {
                // Regular employee
                leadsQuery = leadsQuery.Where(t => t.SalesRepId == currentUserId);
            }

            // Search condition
            leadsQuery = leadsQuery.Where(l =>
                EF.Functions.Like(l.BusinessName, $"%{query}%")
                || EF.Functions.Like(l.WhatsAppNumber, $"%{query}%"));
                //(isId && t.Id == taskId));

            var leads = await leadsQuery
                .OrderByDescending(t => t.Id)
                .ToListAsync();

            return mapper.Map<List<LeadDTO>>(leads);
        }
    }
}
