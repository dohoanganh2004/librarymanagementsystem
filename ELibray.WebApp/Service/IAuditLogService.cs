using E_Library.Models;
using ELibrary.WebApp.Models;

namespace ELibrary.WebApp.Service
{
    public interface IAuditLogService
    {
        Task LogActionAsync(int employeeId, string action, string tableName = null, int? recordId = null, string oldData = null, string newData = null);
        Task<List<AuditLog>> GetAllLogsAsync();
        Task<List<AuditLog>> GetLogsByEmployeeAsync(int employeeId);
        Task<List<AuditLog>> GetLogsByTableAsync(string tableName);
        Task<PaginationModel<AuditLog>> GetPaginatedLogsAsync(int page, int pageSize, string search = null, string tableName = null, int? employeeId = null);
        Task<AuditLog> GetLogByIdAsync(int logId);
    }
}