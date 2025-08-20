using Core.DTOs.Reports;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services.Reporting
{
    public class ReportService : IReportService
    {
        private readonly MinaretOpsDbContext context;
        public ReportService(MinaretOpsDbContext minaret)
        {
            context = minaret;
        }
        public async Task<List<ClientTaskReportDTO>> GetClientTaskReportAsync(int? clientId = null, int? month = null, int? year = null)
        {
            var query = context.Clients
                .Include(c => c.ClientServices)
                .ThenInclude(cs => cs.TaskGroups)
                .ThenInclude(tg => tg.Tasks)
                .ThenInclude(t => t.Employee)
                .Include(c => c.ClientServices)
                .ThenInclude(cs => cs.Service)
                .AsQueryable();

            if (clientId.HasValue)
                query = query.Where(c => c.Id == clientId.Value);

            var clients = await query.ToListAsync();

            var report = new List<ClientTaskReportDTO>();

            foreach (var client in clients)
            {
                var clientReport = new ClientTaskReportDTO
                {
                    ClientId = client.Id,
                    ClientName = client.Name,
                    CompanyName = client.CompanyName ?? "",
                    Status = client.Status,
                    MonthlyTasks = new List<MonthlyTaskGroupDTO>()
                };

                // Group tasks by month
                var monthlyGroups = client.ClientServices
                    .SelectMany(cs => cs.TaskGroups)
                    .Where(tg => !month.HasValue || tg.Month == month.Value)
                    .Where(tg => !year.HasValue || tg.Year == year.Value)
                    .GroupBy(tg => new { tg.Month, tg.Year })
                    .OrderBy(g => g.Key.Year)
                    .ThenBy(g => g.Key.Month);

                foreach (var monthGroup in monthlyGroups)
                {
                    var monthlyReport = new MonthlyTaskGroupDTO
                    {
                        Month = monthGroup.Key.Month,
                        Year = monthGroup.Key.Year,
                        MonthLabel = monthGroup.First().MonthLabel,
                        Tasks = monthGroup
                            .SelectMany(tg => tg.Tasks)
                            .Select(t => new TaskReportDTO
                            {
                                TaskId = t.Id,
                                Title = t.Title,
                                Description = t.Description ?? "",
                                EmployeeName = $"{t.Employee.FirstName} {t.Employee.LastName}",
                                Deadline = t.Deadline,
                                CompletedAt = t.CompletedAt,
                                IsCompletedOnDeadline = t.IsCompletedOnDeadline,
                                Status = t.Status,
                                Priority = t.Priority,
                                Refrence = t.Refrence
                            })
                            .OrderBy(t => t.Deadline)
                            .ToList()
                    };
                    clientReport.MonthlyTasks.Add(monthlyReport);
                }
                report.Add(clientReport);
            }
            return report;
        }
    }
}
