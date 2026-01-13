using E_Library.Models;
using Microsoft.EntityFrameworkCore;

namespace E_Library.Repository
{
    public class AuditLogRepository : IAuditLogRepository
    {
        private readonly ElibraryContext _context;

        public AuditLogRepository(ElibraryContext context)
        {
            _context = context;
        }

        public async Task<List<AuditLog>> GetAllAsync()
        {
            return await _context.AuditLogs
                .Include(a => a.Employee)
                .OrderByDescending(a => a.Timestamp)
                .ToListAsync();
        }

        public async Task<List<AuditLog>> GetByEmployeeIdAsync(int employeeId)
        {
            return await _context.AuditLogs
                .Include(a => a.Employee)
                .Where(a => a.EmployeeId == employeeId)
                .OrderByDescending(a => a.Timestamp)
                .ToListAsync();
        }

        public async Task<List<AuditLog>> GetByTableNameAsync(string tableName)
        {
            return await _context.AuditLogs
                .Include(a => a.Employee)
                .Where(a => a.TableName == tableName)
                .OrderByDescending(a => a.Timestamp)
                .ToListAsync();
        }

        public async Task<List<AuditLog>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.AuditLogs
                .Include(a => a.Employee)
                .Where(a => a.Timestamp >= startDate && a.Timestamp <= endDate)
                .OrderByDescending(a => a.Timestamp)
                .ToListAsync();
        }

        public async Task<AuditLog> GetByIdAsync(int logId)
        {
            return await _context.AuditLogs
                .Include(a => a.Employee)
                .FirstOrDefaultAsync(a => a.LogId == logId);
        }

        public async Task CreateAsync(AuditLog auditLog)
        {
            auditLog.Timestamp = DateTime.Now;
            _context.AuditLogs.Add(auditLog);
            await _context.SaveChangesAsync();
        }

        public async Task<(List<AuditLog> logs, int totalCount)> GetPaginatedAsync(int page, int pageSize, string? search = null, string? tableName = null, int? employeeId = null)
        {
            var query = _context.AuditLogs
                .Include(a => a.Employee)
                .AsQueryable();

            // Apply filters
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(a => a.Description.Contains(search) || 
                                        a.Employee.FirstName.Contains(search) || 
                                        a.Employee.LastName.Contains(search));
            }

            if (!string.IsNullOrEmpty(tableName))
            {
                query = query.Where(a => a.TableName == tableName);
            }

            if (employeeId.HasValue)
            {
                query = query.Where(a => a.EmployeeId == employeeId.Value);
            }

            var totalCount = await query.CountAsync();

            var logs = await query
                .OrderByDescending(a => a.Timestamp)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (logs, totalCount);
        }
    }
}