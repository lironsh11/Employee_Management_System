using Newtonsoft.Json;
using System.Net;

namespace EmployeeManagementSystem.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;
        private readonly IWebHostEnvironment _environment;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger, IWebHostEnvironment environment)
        {
            _next = next;
            _logger = logger;
            _environment = environment;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            // Log the exception
            var errorId = Guid.NewGuid().ToString();
            _logger.LogError(exception, "An unhandled exception occurred. ErrorId: {ErrorId}", errorId);

            // Log to file
            await LogToFileAsync(exception, errorId, context);

            // Return user-friendly error response
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "text/html";

            var isDevelopment = _environment.IsDevelopment();
            var errorMessage = isDevelopment ? exception.ToString() : "An error occurred while processing your request.";

            var errorHtml = $@"
            <!DOCTYPE html>
            <html>
            <head>
                <title>Error</title>
                <style>
                    body {{ font-family: Arial, sans-serif; margin: 40px; }}
                    .error-container {{ max-width: 600px; margin: 0 auto; }}
                    .error-header {{ color: #d32f2f; font-size: 24px; margin-bottom: 20px; }}
                    .error-id {{ color: #666; font-size: 14px; margin-top: 20px; }}
                    .error-details {{ background: #f5f5f5; padding: 15px; border-radius: 4px; margin-top: 20px; }}
                    .back-link {{ display: inline-block; margin-top: 20px; color: #1976d2; text-decoration: none; }}
                    .back-link:hover {{ text-decoration: underline; }}
                </style>
            </head>
            <body>
                <div class='error-container'>
                    <div class='error-header'>Oops! Something went wrong</div>
                    <p>We're sorry, but an error occurred while processing your request.</p>
                    {(isDevelopment ? $"<div class='error-details'><pre>{errorMessage}</pre></div>" : "")}
                    <div class='error-id'>Error ID: {errorId}</div>
                    <a href='/' class='back-link'>← Back to Home</a>
                </div>
            </body>
            </html>";

            await context.Response.WriteAsync(errorHtml);
        }

        private async Task LogToFileAsync(Exception exception, string errorId, HttpContext context)
        {
            try
            {
                var logDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Logs");
                Directory.CreateDirectory(logDirectory);

                var logFileName = $"log-{DateTime.Now:yyyy-MM-dd}.json";
                var logFilePath = Path.Combine(logDirectory, logFileName);

                var logEntry = new
                {
                    ErrorId = errorId,
                    Timestamp = DateTime.UtcNow,
                    Message = exception.Message,
                    StackTrace = exception.StackTrace,
                    Source = exception.Source,
                    RequestPath = context.Request.Path,
                    RequestMethod = context.Request.Method,
                    UserAgent = context.Request.Headers["User-Agent"].ToString(),
                    IPAddress = context.Connection.RemoteIpAddress?.ToString()
                };

                var logJson = JsonConvert.SerializeObject(logEntry, Formatting.Indented);

                // Append to daily log file
                var logEntries = new List<object>();
                if (File.Exists(logFilePath))
                {
                    var existingContent = await File.ReadAllTextAsync(logFilePath);
                    if (!string.IsNullOrEmpty(existingContent))
                    {
                        try
                        {
                            logEntries = JsonConvert.DeserializeObject<List<object>>(existingContent) ?? new List<object>();
                        }
                        catch
                        {
                            // If existing file is corrupted, start fresh
                            logEntries = new List<object>();
                        }
                    }
                }

                logEntries.Add(logEntry);
                var allLogsJson = JsonConvert.SerializeObject(logEntries, Formatting.Indented);
                await File.WriteAllTextAsync(logFilePath, allLogsJson);
            }
            catch (Exception logException)
            {
                _logger.LogError(logException, "Failed to log exception to file");
            }
        }
    }
}