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
            //IQueryable<SalesLead> lead = context.SalesLeads
            //    .AsNoTracking()
            //    .Include(x => x.SalesRep)
            //    .Include(x => x.CreatedBy)
            //    .Include(x => x.Notes)
            //        .ThenInclude(n => n.CreatedBy)
            //    .Include(x => x.ServicesInterestedIn)
            //        .ThenInclude()
            var lead = await context.SalesLeads
                .AsSingleQuery()
               .Include(x => x.SalesRep)
               .Include(x => x.CreatedBy)
               .Include(x => x.Notes)
                   //.ThenInclude(x => x.CreatedBy)
               .Include(x => x.ServicesInterestedIn)
                   .ThenInclude(ls => ls.Service)
               .SingleAsync(x => x.Id == id);
                //?? throw new KeyNotFoundException($"Lead with ID {id} not found.");

            return mapper.Map<LeadDTO>(lead);
        }
        public async Task<LeadDTO> UpdateLeadAsync(UpdateLeadDTO updateLeadDTO)
        {
             var lead = await context.SalesLeads
                .Include(x => x.ServicesInterestedIn)
                .Include(x => x.SalesRep) // Include SalesRep for email if needed
                .SingleAsync(x => x.Id == updateLeadDTO.Id)
                ?? throw new KeyNotFoundException($"Lead with ID {updateLeadDTO.Id} not found.");

            // Update simple properties
            if (!string.IsNullOrEmpty(updateLeadDTO.BusinessName)) lead.BusinessName = updateLeadDTO.BusinessName;
            if (!string.IsNullOrEmpty(updateLeadDTO.WhatsAppNumber)) lead.WhatsAppNumber = updateLeadDTO.WhatsAppNumber;

            lead.Country = updateLeadDTO.Country;
            lead.Occupation = updateLeadDTO.Occupation;
            lead.ContactStatus = updateLeadDTO.ContactStatus;
            lead.CurrentLeadStatus = updateLeadDTO.CurrentLeadStatus;
            lead.LeadSource = updateLeadDTO.LeadSource;
            lead.InterestLevel = updateLeadDTO.InterestLevel;
            lead.Responsibility = updateLeadDTO.Responsibility;
            lead.Budget = updateLeadDTO.Budget;
            lead.Timeline = updateLeadDTO.Timeline;
            lead.NeedsExpectation = updateLeadDTO.NeedsExpectation;
            lead.MeetingDate = updateLeadDTO.MeetingDate;
            lead.QuotationSent = updateLeadDTO.QuotationSent;
            lead.FollowUpTime = updateLeadDTO.FollowUpTime;

            if (updateLeadDTO.LeadSource == LeadSource.FreelancingPlatforms)
            {
                lead.FreelancePlatform = updateLeadDTO.FreelancePlatform;
            }

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
