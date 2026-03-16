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

        //public async Task<List<LeadEmployeeReportDTO>> GetLeadsEmployeeReportAsync(string currentUserId, DateTime? fromDate = null, DateTime? toDate = null)
        //{
        //    var egyptToday = TimeZoneHelper.GetEgyptToday();

        //    DateOnly fromDateOnly;
        //    DateOnly toDateOnly;

        //    if (fromDate.HasValue && toDate.HasValue)
        //    {
        //        fromDateOnly = DateOnly.FromDateTime(fromDate.Value);
        //        toDateOnly = DateOnly.FromDateTime(toDate.Value);
        //    }
        //    else if (fromDate.HasValue)
        //    {
        //        fromDateOnly = DateOnly.FromDateTime(fromDate.Value);
        //        toDateOnly = fromDateOnly;
        //    }
        //    else if (toDate.HasValue)
        //    {
        //        toDateOnly = DateOnly.FromDateTime(toDate.Value);
        //        fromDateOnly = toDateOnly;
        //    }
        //    else
        //    {
        //        fromDateOnly = egyptToday;
        //        toDateOnly = egyptToday;
        //    }

        //    var fromEgyptStart = fromDateOnly.ToDateTime(TimeOnly.MinValue);
        //    var toEgyptEnd = toDateOnly.ToDateTime(TimeOnly.MaxValue);
        //    var fromUtc = TimeZoneInfo.ConvertTimeToUtc(fromEgyptStart, TimeZoneHelper.EgyptTimeZone);
        //    var toUtc = TimeZoneInfo.ConvertTimeToUtc(toEgyptEnd, TimeZoneHelper.EgyptTimeZone);

        //    var leadsData = await context.SalesLeads
        //        .Include(l => l.SalesRep)
        //        .Include(l => l.ServicesInterestedIn)
        //            .ThenInclude(ls => ls.Service)
        //        .Include(l => l.CreatedBy)
        //        .Where(l => l.CreatedAt >= fromUtc && l.CreatedAt <= toUtc)
        //        .ToListAsync();

        //    var salesReps = leadsData
        //        .Where(l => l.SalesRep != null)
        //        .Select(l => l.SalesRep!)
        //        .DistinctBy(u => u.Id)
        //        .ToList();

        //    var reports = new List<LeadEmployeeReportDTO>();

        //    foreach (var rep in salesReps)
        //    {
        //        var repLeads = leadsData.Where(l => l.SalesRepId == rep.Id).ToList();

        //        var report = new LeadEmployeeReportDTO
        //        {
        //            EmployeeId = rep.Id,
        //            EmployeeName = $"{rep.FirstName} {rep.LastName}",
        //            TotalAssignedLeads = repLeads.Count,
        //            MeetingAgreedCount = repLeads.Count(l => l.CurrentLeadStatus == CurrentLeadStatus.MeetingAgreed),
        //            QuotationSentCount = repLeads.Count(l => l.QuotationSent),
        //            DealCount = repLeads.Count(l => l.CurrentLeadStatus == CurrentLeadStatus.Deal),
        //            Leads = mapper.Map<List<LeadDTO>>(repLeads)
        //        };

        //        reports.Add(report);
        //    }

        //    return reports.OrderBy(r => r.EmployeeName).ToList();
        //}

        public async Task<List<LeadEmployeeReportDTO>> GetLeadsEmployeeReportAsync(
    string currentUserId, DateTime? fromDate = null, DateTime? toDate = null)
        {
            var egyptToday = TimeZoneHelper.GetEgyptToday();

            DateOnly fromDateOnly = fromDate.HasValue ? DateOnly.FromDateTime(fromDate.Value) : egyptToday;
            DateOnly toDateOnly = toDate.HasValue ? DateOnly.FromDateTime(toDate.Value) : egyptToday;

            var fromUtc = TimeZoneInfo.ConvertTimeToUtc(fromDateOnly.ToDateTime(TimeOnly.MinValue), TimeZoneHelper.EgyptTimeZone);
            var toUtc = TimeZoneInfo.ConvertTimeToUtc(toDateOnly.ToDateTime(TimeOnly.MaxValue), TimeZoneHelper.EgyptTimeZone);

            // 1. Pull aggregates directly — no Include, no cartesian product
            var reports = await context.SalesLeads
                .Where(l => l.CreatedAt >= fromUtc && l.CreatedAt <= toUtc && l.SalesRepId != null)
                .GroupBy(l => new { l.SalesRepId, l.SalesRep!.FirstName, l.SalesRep.LastName })
                .Select(g => new LeadEmployeeReportDTO
                {
                    EmployeeId = g.Key.SalesRepId!,
                    EmployeeName = g.Key.FirstName + " " + g.Key.LastName,
                    TotalAssignedLeads = g.Count(),
                    MeetingAgreedCount = g.Count(l => l.CurrentLeadStatus == CurrentLeadStatus.MeetingAgreed),
                    QuotationSentCount = g.Count(l => l.QuotationSent),
                    DealCount = g.Count(l => l.CurrentLeadStatus == CurrentLeadStatus.Deal),
                })
                .OrderBy(r => r.EmployeeName)
                .ToListAsync();

            // 2. Load the lead detail rows separately — only what's needed for LeadDTO
            var repIds = reports.Select(r => r.EmployeeId).ToList();

            var leads = await context.SalesLeads
                .Where(l => l.CreatedAt >= fromUtc && l.CreatedAt <= toUtc && repIds.Contains(l.SalesRepId!))
                .Include(l => l.ServicesInterestedIn).ThenInclude(ls => ls.Service)
                .Select(l => new { l.SalesRepId, Lead = l })  // project if LeadDTO doesn't need everything
                .ToListAsync();

            // 3. Stitch in memory — now trivial because data is already grouped
            var leadsByRep = leads.GroupBy(x => x.SalesRepId).ToDictionary(g => g.Key, g => g.Select(x => x.Lead).ToList());

            foreach (var report in reports)
                report.Leads = mapper.Map<List<LeadDTO>>(leadsByRep.GetValueOrDefault(report.EmployeeId, []));

            return reports;
        }
    }
}
