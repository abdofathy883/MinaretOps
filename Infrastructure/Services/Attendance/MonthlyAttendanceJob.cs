using Application.Interfaces;
using Microsoft.Extensions.Logging;
using Quartz;

namespace Infrastructure.Services.Attendance
{
    public class MonthlyAttendanceJob : IJob
    {
        private readonly IAttendanceService attendanceService;
        private readonly ILogger<MonthlyAttendanceJob> logger;

        public MonthlyAttendanceJob(IAttendanceService attendanceService, ILogger<MonthlyAttendanceJob> logger)
        {
            this.attendanceService = attendanceService;
            this.logger = logger;
        }

        public Task Execute(IJobExecutionContext context)
        {
            throw new NotImplementedException();
        }
    }
}
