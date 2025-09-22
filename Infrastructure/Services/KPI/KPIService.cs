using AutoMapper;
using Core.DTOs.KPI;
using Core.Enums;
using Core.Interfaces;
using Core.Models;
using Infrastructure.Data;
using Infrastructure.Exceptions;
using Infrastructure.Services.MediaUploads;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services.KPI
{
    public class KPIService : IKPIService
    {
        private readonly MinaretOpsDbContext context;
        private readonly IEmailService emailService;
        private readonly MediaUploadService mediaUploadService;
        private readonly IMapper mapper;
        public KPIService(
            MinaretOpsDbContext minaret,
            IEmailService email,
            MediaUploadService uploadService,
            IMapper _mapper
            )
        {
            context = minaret;
            emailService = email;
            mediaUploadService = uploadService;
            mapper = _mapper;
        }

        private static readonly Dictionary<KPIAspectType, int> AspectCaps = new()
        {
            { KPIAspectType.Commitment, 30 },
            { KPIAspectType.Productivity, 20 },
            { KPIAspectType.Cooperation, 20 },
            { KPIAspectType.QualityOfWork, 20 },
            { KPIAspectType.CustomerSatisfaction, 10 }
        };
        // Calculates one employee's KPI breakdown for a specific month (resets monthly by filtering incidents to that month only)
        public Task<EmployeeMonthlyKPIDTO> GetEmployeeMonthlyAsync(string employeeId)
        {
            var currentMonth = DateTime.Now.Month;
            var currentyear = DateTime.Now.Year;
            var from = new DateTime(currentyear, currentMonth, 1);
            var to = from.AddMonths(1);

            var employee = context.Users.Find(employeeId)
                ?? throw new InvalidObjectException("Employee not found");

            var incedints = context.KPIIncedints
                .Where(i => i.EmployeeId == employeeId && i.TimeStamp >= from && i.TimeStamp < to)
                .ToList();

            var dto = new EmployeeMonthlyKPIDTO
            {
                EmployeeId = employee.Id,
                EmployeeName = $"{employee.FirstName} {employee.LastName}",
                Year = currentyear,
                // Persist the queried month (used by MonthLabel)
                Month = currentMonth,
                Commitment = AspectCaps[KPIAspectType.Commitment],
                Productivity = AspectCaps[KPIAspectType.Productivity],
                QualityOfWork = AspectCaps[KPIAspectType.QualityOfWork],
                Cooperation = AspectCaps[KPIAspectType.Cooperation],
                CustomerSatisfaction = AspectCaps[KPIAspectType.CustomerSatisfaction]
            };

            // For each aspect, apply capped deduction: min(incidents * 10%, AspectCap)
            foreach (var g in incedints.GroupBy(i => i.Aspect))
            {
                // Resolve cap for this aspect (fallback to 20% if not configured)
                var cap = AspectCaps.TryGetValue(g.Key, out var c) ? c : 20;
                // Each incident deducts 10%; cap the deduction by the configured limit
                var deduction = Math.Min(g.Count() * 10, cap);

                // Subtract the effective deduction from the corresponding aspect, never below 0
                switch (g.Key)
                {
                    // Apply deduction to Commitment
                    case KPIAspectType.Commitment:
                        dto.Commitment = Math.Max(0, dto.Commitment - deduction);
                        break;
                    // Apply deduction to Productivity
                    case KPIAspectType.Productivity:
                        dto.Productivity = Math.Max(0, dto.Productivity - deduction);
                        break;
                    // Apply deduction to QualityOfWork
                    case KPIAspectType.QualityOfWork:
                        dto.QualityOfWork = Math.Max(0, dto.QualityOfWork - deduction);
                        break;
                    // Apply deduction to Cooperation
                    case KPIAspectType.Cooperation:
                        dto.Cooperation = Math.Max(0, dto.Cooperation - deduction);
                        break;
                    // Apply deduction to CustomerSatisfaction
                    case KPIAspectType.CustomerSatisfaction:
                        dto.CustomerSatisfaction = Math.Max(0, dto.CustomerSatisfaction - deduction);
                        break;
                }
            }
            return Task.FromResult(dto);
        }

        public async Task<List<IncedintDTO>> GetIncedientsByEmpIdAsync(string employeeId)
        {
            var incedients = await context.KPIIncedints
                .Where(i => i.EmployeeId == employeeId)
                .ToListAsync();

            return mapper.Map<List<IncedintDTO>>(incedients);
        }

        public async Task<List<EmployeeMonthlyKPIDTO>> GetMonthlySummeriesAsync()
        {
            var employeeIds = await context.Users.Select(u => u.Id).ToListAsync();
            var summaries = new List<EmployeeMonthlyKPIDTO>(employeeIds.Count);
            foreach (var id in employeeIds)
                summaries.Add(await GetEmployeeMonthlyAsync(id));
            return summaries.OrderBy(x => x.EmployeeName).ToList();
        }

        public async Task<IncedintDTO> NewKPIIncedintAsync(CreateIncedintDTO dto)
        {
            if (dto is null)
                throw new InvalidObjectException("Invalid data");

            var employee = await context.Users.FindAsync(dto.EmployeeId)
                ?? throw new InvalidObjectException("Employee not found");

            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                var evidenceURL = string.Empty;
                if(dto.EvidenceURL is not null)
                {
                    var uploaded = await mediaUploadService.UploadImageWithPath(
                        dto.EvidenceURL,
                        $"{employee.FirstName} {employee.LastName} - {dto.Aspect} {DateTime.UtcNow}"
                        );
                    evidenceURL = uploaded.Url;
                }
                var incedint = new KPIIncedint
                {
                    EmployeeId = dto.EmployeeId,
                    Aspect = dto.Aspect,
                    Description = dto.Description,
                    EvidenceURL = evidenceURL,
                    TimeStamp = DateTime.UtcNow
                };
                await context.KPIIncedints.AddAsync(incedint);
                await context.SaveChangesAsync();

                Dictionary<string, string> replacements = new Dictionary<string, string>
                {
                    { "{{EmployeeName}}", $"{employee.FirstName} {employee.LastName}" },
                    { "{{Aspect}}", dto.Aspect.ToString() },
                    { "{{Description}}", dto.Description ?? "N/A" },
                    { "{{TimeStamp}}", incedint.TimeStamp.ToString("f") },
                    { "{{PenaltyPercentage}}", incedint.PenaltyPercentage.ToString() },
                    { "{{EvidenceURL}}", incedint.EvidenceURL ?? "N/A" }
                };

                //await emailService.SendEmailWithTemplateAsync(employee.Email ?? string.Empty, "incedint", "KPIIncedint", replacements);
                await transaction.CommitAsync();
                return mapper.Map<IncedintDTO>(incedint);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception($"Failed to create incedint: {ex.Message}" );
            }
        }

        public async Task<List<IncedintDTO>> GetAllIncedientsAsync()
        {
            var incedeients = await context.KPIIncedints
                .Include(i => i.Employee)
                .ToListAsync();
            return mapper.Map<List<IncedintDTO>>(incedeients);
        }
    }
}
