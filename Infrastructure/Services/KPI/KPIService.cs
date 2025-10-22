using AutoMapper;
using Core.DTOs.KPI;
using Core.Enums;
using Core.Interfaces;
using Core.Models;
using Infrastructure.Data;
using Infrastructure.Exceptions;
using Infrastructure.Services.MediaUploads;
using Infrastructure.Services.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services.KPI
{
    public class KPIService : IKPIService
    {
        private readonly MinaretOpsDbContext context;
        private readonly TaskHelperService helperService;
        private readonly MediaUploadService mediaUploadService;
        private readonly IMapper mapper;
        public KPIService(
            MinaretOpsDbContext minaret,
            TaskHelperService _helperService,
            MediaUploadService uploadService,
            IMapper _mapper
            )
        {
            context = minaret;
            helperService = _helperService;
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
        public async Task<EmployeeMonthlyKPIDTO> GetEmployeeMonthlyAsync(string employeeId, int? month = null, int? year = null)
        {
            var currentMonth = month ?? DateTime.Now.Month;
            var currentyear = year ?? DateTime.Now.Year;
            var from = new DateTime(currentyear, currentMonth, 1);
            var to = from.AddMonths(1);

            var employee = await context.Users.FindAsync(employeeId)
                ?? throw new InvalidObjectException("لم يتم العثور على الموظف");

            var incedints = await context.KPIIncedints
                .Where(i => i.EmployeeId == employeeId && i.TimeStamp >= from && i.TimeStamp < to)
                .ToListAsync();

            var dto = new EmployeeMonthlyKPIDTO
            {
                EmployeeId = employee.Id,
                EmployeeName = $"{employee.FirstName} {employee.LastName}",
                Year = currentyear,
                Month = currentMonth,
                Commitment = AspectCaps[KPIAspectType.Commitment],
                Productivity = AspectCaps[KPIAspectType.Productivity],
                QualityOfWork = AspectCaps[KPIAspectType.QualityOfWork],
                Cooperation = AspectCaps[KPIAspectType.Cooperation],
                CustomerSatisfaction = AspectCaps[KPIAspectType.CustomerSatisfaction]
            };

            foreach (var g in incedints.GroupBy(i => i.Aspect))
            {
                var cap = AspectCaps.TryGetValue(g.Key, out var c) ? c : 20;
                var deduction = Math.Min(g.Count() * 10, cap);

                switch (g.Key)
                {
                    case KPIAspectType.Commitment:
                        dto.Commitment = Math.Max(0, dto.Commitment - deduction);
                        break;
                    case KPIAspectType.Productivity:
                        dto.Productivity = Math.Max(0, dto.Productivity - deduction);
                        break;
                    case KPIAspectType.QualityOfWork:
                        dto.QualityOfWork = Math.Max(0, dto.QualityOfWork - deduction);
                        break;
                    case KPIAspectType.Cooperation:
                        dto.Cooperation = Math.Max(0, dto.Cooperation - deduction);
                        break;
                    case KPIAspectType.CustomerSatisfaction:
                        dto.CustomerSatisfaction = Math.Max(0, dto.CustomerSatisfaction - deduction);
                        break;
                }
            }
            return dto;
        }
        public async Task<List<IncedintDTO>> GetIncedientsByEmpIdAsync(string employeeId)
        {
            var incedients = await context.KPIIncedints
                .Where(i => i.EmployeeId == employeeId)
                .ToListAsync();

            return mapper.Map<List<IncedintDTO>>(incedients);
        }
        public async Task<List<EmployeeMonthlyKPIDTO>> GetMonthlySummeriesAsync(int? month = null, int? year = null)
        {
            var employeeIds = await context.Users.Select(u => u.Id).ToListAsync();
            var summaries = new List<EmployeeMonthlyKPIDTO>(employeeIds.Count);
            foreach (var id in employeeIds)
                summaries.Add(await GetEmployeeMonthlyAsync(id, month, year));
            return summaries.OrderBy(x => x.EmployeeName).ToList();
        }
        public async Task<IncedintDTO> NewKPIIncedintAsync(CreateIncedintDTO dto)
        {
            var employee = await helperService.GetUserOrThrow(dto.EmployeeId)
                ?? throw new InvalidObjectException("الموظف غير موجود");

            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                var evidenceURL = string.Empty;
                if(dto.EvidenceURL is not null)
                {
                    var uploaded = await mediaUploadService.UploadImageWithPath(
                        dto.EvidenceURL,
                        $"{employee.FirstName}_{employee.LastName}-{dto.Aspect}"
                        );
                    evidenceURL = uploaded.Url;
                }
                var incedint = new KPIIncedint
                {
                    EmployeeId = employee.Id,
                    Aspect = dto.Aspect,
                    Description = dto.Description,
                    EvidenceURL = evidenceURL,
                    TimeStamp = DateTime.UtcNow,
                    Date = dto.Date
                };
                await context.KPIIncedints.AddAsync(incedint);

                if (!string.IsNullOrEmpty(employee.Email))
                {
                    var emailPayload = new
                    {
                        To = employee.Email,
                        Subject = "New KPI Incedient",
                        Template = "NewIncedient",
                        Replacements = new Dictionary<string, string>
                        {
                            { "{{EmployeeName}}", $"{employee.FirstName} {employee.LastName}" },
                            { "{{Aspect}}", dto.Aspect.ToString() },
                            { "{{Description}}", dto.Description ?? "N/A" },
                            { "{{TimeStamp}}", incedint.TimeStamp.ToString("f") },
                            { "{{PenaltyPercentage}}", incedint.PenaltyPercentage.ToString() },
                            { "{{EvidenceURL}}", incedint.EvidenceURL ?? "N/A" }
                        }
                    };
                    await helperService.AddOutboxAsync(OutboxTypes.Email, "KPI Incedient Email", emailPayload);
                }
                await context.SaveChangesAsync();
                await transaction.CommitAsync();
                return mapper.Map<IncedintDTO>(incedint);
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
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
