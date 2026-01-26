using AutoMapper;
using Core.DTOs.Leads;
using Core.Enums;
using Core.Interfaces;
using Core.Models;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;

namespace Infrastructure.Services.Leads
{
    public class LeadService : ILeadService
    {
        private readonly MinaretOpsDbContext context;
        // private readonly IEmailService emailService; 
        private readonly IMapper mapper;

        public LeadService(MinaretOpsDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<LeadDTO> CreateLeadAsync(CreateLeadDTO createLeadDTO)
        {
            var lead = mapper.Map<SalesLead>(createLeadDTO);
            lead.CreatedAt = DateTime.UtcNow;
            
            // map Services manually if mapper doesn't fully handle the complex list creation/attach
            // AutoMapper in LeadProfile handles this now


            context.SalesLeads.Add(lead);
            await context.SaveChangesAsync();
            return await GetLeadByIdAsync(lead.Id); // Return full object with includes
        }

        public Task<bool> DeleteLeadAsync(Guid leadId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<LeadDTO>> GetAllLeadsAsync()
        {
            var leads = await context.SalesLeads
                .Include(x => x.SalesRep)
                .Include(x => x.CreatedBy)
                .Include(x => x.ServicesInterestedIn)
                    .ThenInclude(ls => ls.Service)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
            return mapper.Map<List<LeadDTO>>(leads);
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

        public Task<LeadDTO> UpdateLeadAsync(UpdateLeadDTO updateLeadDTO)
        {
             throw new NotImplementedException();
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
    }
}
