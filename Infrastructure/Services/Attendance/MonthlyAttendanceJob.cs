using Core.Interfaces;
using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
