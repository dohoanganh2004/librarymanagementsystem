using E_Library.Models;

namespace E_Library.Repository
{
    public interface IAuditLogRepository
    {
        Task<List<AuditLog>> GetAllAsync();
        Task<List<AuditLog>> GetByEmployeeIdAsync(int employeeId);
        Task<List<AuditLog>> GetByTableNameAsync(string tableName);
        Task<List<AuditLog>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<AuditLog> GetByIdAsync(int logId);
        Task CreateAsync(AuditLog auditLog);
        Task<(List<AuditLog> logs, int totalCount)> GetPaginatedAsync(int page, int pageSize, string? search = null, string? tableName = null, int? employeeId = null);
    }
}