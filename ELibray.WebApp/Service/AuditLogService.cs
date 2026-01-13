using E_Library.Models;
using E_Library.Repository;
using ELibrary.WebApp.Models;

namespace ELibrary.WebApp.Service
{
    public class AuditLogService : IAuditLogService
    {
        private readonly IAuditLogRepository _auditLogRepository;
        private readonly ILogger<AuditLogService> _logger;

        public AuditLogService(IAuditLogRepository auditLogRepository, ILogger<AuditLogService> logger)
        {
            _auditLogRepository = auditLogRepository;
            _logger = logger;
        }

        public async Task LogActionAsync(int employeeId, string action, string tableName = null, int? recordId = null, string oldData = null, string newData = null)
        {
            try
            {
                var auditLog = new AuditLog
                {
                    EmployeeId = employeeId,
                    Description = action,
                    TableName = tableName,
                    RecordId = recordId,
                    OldData = oldData,
                    NewData = newData,
                    Timestamp = DateTime.Now
                };

                await _auditLogRepository.CreateAsync(auditLog);
                _logger.LogInformation($"Audit log created: {action} by Employee {employeeId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to create audit log for Employee {employeeId}: {action}");
            }
        }

        public async Task<List<AuditLog>> GetAllLogsAsync()
        {
            return await _auditLogRepository.GetAllAsync();
        }

        public async Task<List<AuditLog>> GetLogsByEmployeeAsync(int employeeId)
        {
            return await _auditLogRepository.GetByEmployeeIdAsync(employeeId);
        }

        public async Task<List<AuditLog>> GetLogsByTableAsync(string tableName)
        {
            return await _auditLogRepository.GetByTableNameAsync(tableName);
        }

        public async Task<PaginationModel<AuditLog>> GetPaginatedLogsAsync(int page, int pageSize, string search = null, string tableName = null, int? employeeId = null)
        {
            var (logs, totalCount) = await _auditLogRepository.GetPaginatedAsync(page, pageSize, search, tableName, employeeId);
            
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            return new PaginationModel<AuditLog>
            {
                Items = logs,
                CurrentPage = page,
                TotalPages = totalPages,
                TotalItems = totalCount,
                PageSize = pageSize
            };
        }

        public async Task<AuditLog> GetLogByIdAsync(int logId)
        {
            return await _auditLogRepository.GetByIdAsync(logId);
        }
    }
}