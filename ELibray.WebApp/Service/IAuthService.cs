using E_Library.Models;
using ELibrary.WebApp.DTO.Request;
using Microsoft.AspNetCore.Identity.Data;

namespace ELibrary.WebApp.Service
{
    public interface IAuthService
    {
        Task<Reader?> LoginReader(String email, String Password);
        Task<Employee?> LoginEmployee(String email, String Password);
        Task<Reader> Register(ReaderRequestDTO readerRequestDTO);
        Task<bool> ForgotPassword(String email);
        
        // ✅ THÊM MỚI: Method kiểm tra user tồn tại
        Task<bool> IsUserExist(String email);
    }
}
