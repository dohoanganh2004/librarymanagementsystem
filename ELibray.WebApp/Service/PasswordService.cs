using ELibrary.WebApp.DTO.Request;
using ELibrary.WebApp.Security;
using E_Library.Models;
using Microsoft.EntityFrameworkCore;

namespace ELibrary.WebApp.Service
{
    public class PasswordService : IPasswordService
    {
        private readonly ElibraryContext _context;
        private readonly ILogger<PasswordService> _logger;

        public PasswordService(ElibraryContext context, ILogger<PasswordService> logger)
        {
            _context = context;
            _logger = logger;
        }

      
        public async Task<bool> ChangeReaderPasswordAsync(int readerId, ChangePasswordRequestDTO request)
        {
            try
            {
                var reader = await _context.Readers.FindAsync(readerId);
                if (reader == null) return false;

              
                if (!PasswordHelper.VerifyPassword(request.CurrentPassword, reader.Password))
                    return false;

               
                reader.Password = PasswordHelper.HashPassword(request.NewPassword);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ChangeReaderPassword failed: {readerId}");
                return false;
            }
        }

     
        public async Task<bool> ChangeEmployeePasswordAsync(int employeeId, ChangePasswordRequestDTO request)
        {
            try
            {
                var employee = await _context.Employees.FindAsync(employeeId);
                if (employee == null) return false;

                if (!PasswordHelper.VerifyPassword(request.CurrentPassword, employee.Password))
                    return false;

                employee.Password = PasswordHelper.HashPassword(request.NewPassword);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ChangeEmployeePassword failed: {employeeId}");
                return false;
            }
        }

        
        public async Task<bool> ValidateCurrentPasswordAsync(int userId, string currentPassword, string userType)
        {
            if (userType.ToLower() == "reader")
            {
                var reader = await _context.Readers.FindAsync(userId);
                return reader != null &&
                       PasswordHelper.VerifyPassword(currentPassword, reader.Password);
            }

            if (userType.ToLower() == "employee")
            {
                var employee = await _context.Employees.FindAsync(userId);
                return employee != null &&
                       PasswordHelper.VerifyPassword(currentPassword, employee.Password);
            }

            return false;
        }

        
        public async Task<bool> ResetPasswordAsync(int userId, string userType, string newPassword)
        {
            var newHash = PasswordHelper.HashPassword(newPassword);

            if (userType.ToLower() == "reader")
            {
                var reader = await _context.Readers.FindAsync(userId);
                if (reader == null) return false;

                reader.Password = newHash;
            }
            else if (userType.ToLower() == "employee")
            {
                var employee = await _context.Employees.FindAsync(userId);
                if (employee == null) return false;

                employee.Password = newHash;
            }
            else
            {
                return false;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public Task LogPasswordChangeAsync(int userId, string userType, string ipAddress)
        {
            _logger.LogInformation(
                $"Password changed | {userType} | UserId={userId} | IP={ipAddress} | Time={DateTime.Now}"
            );
            return Task.CompletedTask;
        }
    }
}
