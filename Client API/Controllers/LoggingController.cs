using Core.DTOs.LoggingDTO;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Serilog.Events;

namespace Client_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoggingController : ControllerBase
    {
        [HttpPost("logs")]
        public IActionResult LogFromFrontend([FromBody] FrontendLogDTO log)
        {
            if (log == null || string.IsNullOrEmpty(log.Level) || string.IsNullOrEmpty(log.Message))
            {
                return BadRequest("Invalid log data");
            }

            Log.ForContext("Source", "Frontend")
               .ForContext("Level", log.Level)
               .Write(log.Level switch
               {
                   "debug" => LogEventLevel.Debug,
                   "info" => LogEventLevel.Information,
                   "warn" => LogEventLevel.Warning,
                   "error" => LogEventLevel.Error,
                   _ => LogEventLevel.Information
               }, "{Message} {@Data}", log.Message, log.Data);

            return Ok();
        }
    }
}
