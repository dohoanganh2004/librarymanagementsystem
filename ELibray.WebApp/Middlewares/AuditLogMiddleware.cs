using ELibrary.WebApp.Service;
using System.Text.Json;

namespace ELibrary.WebApp.Middlewares
{
    public class AuditLogMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<AuditLogMiddleware> _logger;

        public AuditLogMiddleware(RequestDelegate next, ILogger<AuditLogMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, IAuditLogService auditLogService)
        {
            // Skip logging for static files and non-important requests
            var path = context.Request.Path.Value?.ToLowerInvariant();
            if (ShouldSkipLogging(path))
            {
                await _next(context);
                return;
            }

            var employeeId = context.Session.GetInt32("employeeId");
            if (!employeeId.HasValue)
            {
                await _next(context);
                return;
            }

            var startTime = DateTime.Now;
            var method = context.Request.Method;
            var originalBodyStream = context.Response.Body;

            try
            {
                await _next(context);

                // Log successful operations
                if (context.Response.StatusCode >= 200 && context.Response.StatusCode < 300)
                {
                    await LogImportantActions(context, auditLogService, employeeId.Value, method, path, startTime);
                }
            }
            catch (Exception ex)
            {
                // Log failed operations
                await auditLogService.LogActionAsync(
                    employeeId.Value,
                    $"Lỗi khi thực hiện: {method} {path}",
                    "System",
                    null,
                    null,
                    JsonSerializer.Serialize(new
                    {
                        Error = ex.Message,
                        Method = method,
                        Path = path,
                        Timestamp = DateTime.Now
                    })
                );

                throw;
            }
        }

        private bool ShouldSkipLogging(string path)
        {
            if (string.IsNullOrEmpty(path)) return true;

            var skipPaths = new[]
            {
                "/css/", "/js/", "/img/", "/fonts/", "/lib/",
                "/favicon.ico", "/auditlog", "/statistics",
                "/_vs", "/signalr"
            };

            return skipPaths.Any(skipPath => path.Contains(skipPath));
        }

        private async Task LogImportantActions(HttpContext context, IAuditLogService auditLogService, 
            int employeeId, string method, string path, DateTime startTime)
        {
            var duration = (DateTime.Now - startTime).TotalMilliseconds;

            // Only log important actions
            var importantActions = new Dictionary<string, string>
            {
                { "post /book/create", "Truy cập trang tạo sách" },
                { "post /book/update", "Truy cập trang cập nhật sách" },
                { "get /book/delete", "Truy cập xóa sách" },
                { "post /employee/create", "Truy cập tạo nhân viên" },
                { "post /employee/update", "Truy cập cập nhật nhân viên" },
                { "get /employee/delete", "Truy cập xóa nhân viên" },
                { "post /reader/create", "Truy cập tạo độc giả" },
                { "post /reader/update", "Truy cập cập nhật độc giả" },
                { "get /checkout/all", "Truy cập danh sách mượn sách" },
                { "get /reservation/list", "Truy cập danh sách đặt chỗ" },
                { "get /auditlog", "Truy cập log hệ thống" }
            };

            var actionKey = $"{method.ToLower()} {path}";
            if (importantActions.ContainsKey(actionKey))
            {
                await auditLogService.LogActionAsync(
                    employeeId,
                    importantActions[actionKey],
                    "Navigation",
                    null,
                    null,
                    JsonSerializer.Serialize(new
                    {
                        Method = method,
                        Path = path,
                        Duration = $"{duration:F2}ms",
                        Timestamp = DateTime.Now,
                        IPAddress = context.Connection.RemoteIpAddress?.ToString(),
                        UserAgent = context.Request.Headers["User-Agent"].ToString()
                    })
                );
            }
        }
    }
}