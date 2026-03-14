using AutoMapper;
using Core.DTOs.Leads;
using Core.DTOs.Leads.Reports;
using Core.Enums.Leads;
using Core.Interfaces.Leads;
using Core.Models;
using Infrastructure.Data;
using Infrastructure.Helpers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Services.Leads
{
    public class LeadsReportService : ILeadReportService
    {
        private readonly MinaretOpsDbContext context;
        private readonly IMapper mapper;

        public LeadsReportService(MinaretOpsDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<List<LeadEmployeeReportDTO>> GetLeadsEmployeeReportAsync(string currentUserId, DateTime? fromDate = null, DateTime? toDate = null)
        {
            var egyptToday = TimeZoneHelper.GetEgyptToday();
            
            DateOnly fromDateOnly;
            DateOnly toDateOnly;
            
            if (fromDate.HasValue && toDate.HasValue)
            {
                fromDateOnly = DateOnly.FromDateTime(fromDate.Value);
                toDateOnly = DateOnly.FromDateTime(toDate.Value);
            }
            else if (fromDate.HasValue)
            {
                fromDateOnly = DateOnly.FromDateTime(fromDate.Value);
                toDateOnly = fromDateOnly;
            }
            else if (toDate.HasValue)
            {
                toDateOnly = DateOnly.FromDateTime(toDate.Value);
                fromDateOnly = toDateOnly;
            }
            else
            {
                fromDateOnly = egyptToday;
                toDateOnly = egyptToday;
            }

            var fromEgyptStart = fromDateOnly.ToDateTime(TimeOnly.MinValue);
            var toEgyptEnd = toDateOnly.ToDateTime(TimeOnly.MaxValue);
            var fromUtc = TimeZoneInfo.ConvertTimeToUtc(fromEgyptStart, TimeZoneHelper.EgyptTimeZone);
            var toUtc = TimeZoneInfo.ConvertTimeToUtc(toEgyptEnd, TimeZoneHelper.EgyptTimeZone);

            var leadsData = await context.SalesLeads
                .Include(l => l.SalesRep)
                .Include(l => l.ServicesInterestedIn)
                    .ThenInclude(ls => ls.Service)
                .Include(l => l.CreatedBy)
                .Where(l => l.CreatedAt >= fromUtc && l.CreatedAt <= toUtc)
                .ToListAsync();

            var salesReps = leadsData
                .Where(l => l.SalesRep != null)
                .Select(l => l.SalesRep!)
                .DistinctBy(u => u.Id)
                .ToList();

            var reports = new List<LeadEmployeeReportDTO>();

            foreach (var rep in salesReps)
            {
                var repLeads = leadsData.Where(l => l.SalesRepId == rep.Id).ToList();

                var report = new LeadEmployeeReportDTO
                {
                    EmployeeId = rep.Id,
                    EmployeeName = $"{rep.FirstName} {rep.LastName}",
                    TotalAssignedLeads = repLeads.Count,
                    MeetingAgreedCount = repLeads.Count(l => l.CurrentLeadStatus == CurrentLeadStatus.MeetingAgreed),
                    QuotationSentCount = repLeads.Count(l => l.QuotationSent),
                    DealCount = repLeads.Count(l => l.CurrentLeadStatus == CurrentLeadStatus.Deal),
                    Leads = mapper.Map<List<LeadDTO>>(repLeads)
                };

                reports.Add(report);
            }

            return reports.OrderBy(r => r.EmployeeName).ToList();
        }
    }
}
