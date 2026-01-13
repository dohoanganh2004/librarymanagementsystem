using ELibrary.WebApp.DTO.Request;

namespace ELibrary.WebApp.Service
{
    public interface IPasswordService
    {
        Task<bool> ChangeReaderPasswordAsync(int readerId, ChangePasswordRequestDTO request);
        Task<bool> ChangeEmployeePasswordAsync(int employeeId, ChangePasswordRequestDTO request);
        Task<bool> ValidateCurrentPasswordAsync(int userId, string currentPassword, string userType);
        Task<bool> ResetPasswordAsync(int userId, string userType, string newPassword);
        Task LogPasswordChangeAsync(int userId, string userType, string ipAddress);
    }
}