using Core.Interfaces;
using Microsoft.Extensions.Logging;
using Quartz;

namespace Infrastructure.Services.Attendance
{
    public class AttendanceJob : IJob
    {
        private readonly IAttendanceService attendanceService;
        private readonly ILogger<AttendanceJob> _logger;

        public AttendanceJob(
            IAttendanceService service, 
            ILogger<AttendanceJob> logger
            )
        {
            attendanceService = service;
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                _logger.LogInformation("Running daily attendance job at {time}", DateTime.Now);
                await attendanceService.MarkAbsenteesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while running attendance job");
            }
        }
    }
}
