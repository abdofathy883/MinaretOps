using Core.Interfaces.Auth;
using Core.Models;
using System.Security.Claims;
using System.Text.Json;

namespace ClientAPI.Middlewares
{
    public class LoginLoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public LoginLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ILoginLogService loginLogService)
        {
            // Only care about the login endpoint
            if (!context.Request.Path.StartsWithSegments("/api/auth/login") 
                || context.Request.Method != "POST")
            {
                await _next(context);
                return;
            }

            // Enable buffering to re-read the body
            context.Request.EnableBuffering();
            var requestBody = await new StreamReader(context.Request.Body).ReadToEndAsync();
            context.Request.Body.Position = 0; // Reset for the actual handler

            // Buffer the response to capture failure reasons
            var originalBodyStream = context.Response.Body;
            using (var responseBody = new MemoryStream())
            {
                context.Response.Body = responseBody;

                // Extract phone number from request body
                string? phoneNumber = null;
                string? failureReason = null;

                if (!string.IsNullOrEmpty(requestBody))
                {
                    try
                    {
                        var loginDto = JsonSerializer.Deserialize<JsonElement>(requestBody);
                        phoneNumber = loginDto.TryGetProperty("phoneNumber", out var p) ? p.GetString() : null;
                    }
                    catch (JsonException)
                    {
                        // Invalid JSON, continue without phone number
                    }
                }

                // Process the request
                await _next(context);

                // Determine success and capture failure reason
                var isSuccess = context.Response.StatusCode == 200;

                if (!isSuccess)
                {
                    try
                    {
                        context.Response.Body.Seek(0, SeekOrigin.Begin);
                        var responseText = await new StreamReader(context.Response.Body).ReadToEndAsync();
                        
                        if (!string.IsNullOrEmpty(responseText))
                        {
                            var responseJson = JsonSerializer.Deserialize<JsonElement>(responseText);
                            
                            // Try to extract error message from response
                            if (responseJson.TryGetProperty("message", out var msg))
                            {
                                failureReason = msg.GetString();
                            }
                            else if (responseJson.TryGetProperty("error", out var err))
                            {
                                failureReason = err.GetString();
                            }
                        }
                    }
                    catch
                    {
                        failureReason = $"HTTP {context.Response.StatusCode}";
                    }
                }

                // Get IP and User-Agent
                var ip = context.Connection.RemoteIpAddress?.ToString();
                var userAgent = context.Request.Headers["User-Agent"].ToString();

                // Log the login attempt
                await loginLogService.LogAsync(new LoginLog
                {
                    UserId = phoneNumber,
                    IpAddress = ip,
                    IsSuccess = isSuccess,
                    FailureReason = failureReason,
                    UserAgent = userAgent,
                    Timestamp = DateTime.UtcNow
                });

                // Copy buffered response to original stream
                await responseBody.CopyToAsync(originalBodyStream);
            }
        }
    }
}
