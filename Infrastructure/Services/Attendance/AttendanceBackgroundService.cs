using Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services.Attendance
{
    public class AttendanceBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<AttendanceBackgroundService> _logger;

        public AttendanceBackgroundService(
            IServiceProvider serviceProvider, 
            ILogger<AttendanceBackgroundService> logger
            )
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var attendanceService = scope.ServiceProvider.GetRequiredService<IAttendanceService>();

                        await attendanceService.MarkAbsenteesAsync();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while marking absentees");
                }

                // Run every 30 minutes (tune as you like)
                await Task.Delay(TimeSpan.FromMinutes(30), stoppingToken);
            }
        }
    }
}
