using ELibrary.WebApp.Service;
using Microsoft.AspNetCore.Mvc;

namespace ELibrary.WebApp.Controllers
{
    public class AuditLogController : Controller
    {
        private readonly IAuditLogService _auditLogService;
        private readonly ILogger<AuditLogController> _logger;

        public AuditLogController(IAuditLogService auditLogService, ILogger<AuditLogController> logger)
        {
            _auditLogService = auditLogService;
            _logger = logger;
        }

        // GET: AuditLog
        public async Task<IActionResult> Index(int page = 1, int pageSize = 20, string search = null, string tableName = null, int? employeeId = null)
        {
            try
            {
                var paginatedLogs = await _auditLogService.GetPaginatedLogsAsync(page, pageSize, search, tableName, employeeId);
                
                ViewBag.CurrentSearch = search;
                ViewBag.CurrentTableName = tableName;
                ViewBag.CurrentEmployeeId = employeeId;
                ViewBag.PageSize = pageSize;

                return View(paginatedLogs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading audit logs");
                TempData["error"] = "Có lỗi xảy ra khi tải log hệ thống";
                return View();
            }
        }

        // GET: AuditLog/Details/5
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var auditLog = await _auditLogService.GetLogByIdAsync(id);
                if (auditLog == null)
                {
                    return NotFound();
                }

                return View(auditLog);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error loading audit log details for ID: {id}");
                TempData["error"] = "Có lỗi xảy ra khi tải chi tiết log";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: AuditLog/Employee/5
        public async Task<IActionResult> ByEmployee(int employeeId, int page = 1, int pageSize = 20)
        {
            try
            {
                var paginatedLogs = await _auditLogService.GetPaginatedLogsAsync(page, pageSize, null, null, employeeId);
                
                ViewBag.EmployeeId = employeeId;
                ViewBag.PageSize = pageSize;

                return View("Index", paginatedLogs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error loading audit logs for employee: {employeeId}");
                TempData["error"] = "Có lỗi xảy ra khi tải log nhân viên";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: AuditLog/Table/Books
        public async Task<IActionResult> ByTable(string tableName, int page = 1, int pageSize = 20)
        {
            try
            {
                var paginatedLogs = await _auditLogService.GetPaginatedLogsAsync(page, pageSize, null, tableName, null);
                
                ViewBag.TableName = tableName;
                ViewBag.PageSize = pageSize;

                return View("Index", paginatedLogs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error loading audit logs for table: {tableName}");
                TempData["error"] = "Có lỗi xảy ra khi tải log bảng dữ liệu";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}