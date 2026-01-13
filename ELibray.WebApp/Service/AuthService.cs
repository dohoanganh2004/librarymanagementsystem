using System.Security.Cryptography;
using AutoMapper;
using E_Library.Models;
using E_Library.Repository;
using ELibrary.WebApp.DTO.Request;
using ELibrary.WebApp.EmailServices;
using ELibrary.WebApp.Models;
using ELibrary.WebApp.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace ELibrary.WebApp.Service
{
    public class AuthService : IAuthService
    {
        private readonly IReaderRepository readerRepository;
        private readonly IEmployeeRepository employeeRepository;
        private readonly INotificationService notificationService;
        private readonly IMapper mapper;

        public AuthService(
            IReaderRepository readerRepository,
            IMapper mapper,
            IEmployeeRepository employeeRepository,
            INotificationService notificationService)
        {
            this.readerRepository = readerRepository;
            this.mapper = mapper;
            this.employeeRepository = employeeRepository;
            this.notificationService = notificationService;
        }


        // ================================
        // LOGIN READER
        // ================================
        public async Task<Reader?> LoginReader(string email, string password)
        {
            var loginReader = await readerRepository.getReaderByEmail(email);

            if (loginReader == null)
                return null;
            if (loginReader.Status == false) return null;
           
            if (!PasswordHelper.VerifyPassword(password, loginReader.Password))
                return null; 

            return loginReader;
        }


        // ================================
        // LOGIN EMPLOYEE
        // ================================
        public async Task<Employee?> LoginEmployee(string email, string password)
        {
            var loginEmployee = await employeeRepository.GetByEmail(email);

            if (loginEmployee == null)
                return null;
             
            if (loginEmployee.Status == false) return null;
            
            if (!PasswordHelper.VerifyPassword(password, loginEmployee.Password))
                return null; 

            return loginEmployee;
        }


        // ================================
        // REGISTER READER
        // ================================
        public async Task<Reader> Register(ReaderRequestDTO readerRequestDTO)
        {
            var getReaderByEmail = await readerRepository.getReaderByEmail(readerRequestDTO.Email);
            var getReaderByPhoneNumber = await readerRepository.getReaderByPhoneNumber(readerRequestDTO.PhoneNumber);

            if (getReaderByEmail != null)
                throw new Exception("Email already exists. Please choose another email.");

            if (getReaderByPhoneNumber != null)
                throw new Exception("Phone number already exists. Please choose another phone number.");

            readerRequestDTO.Password = PasswordHelper.HashPassword(readerRequestDTO.Password);

            var reader = mapper.Map<Reader>(readerRequestDTO);

            var registerReader = await readerRepository.CreateReader(reader);

            return registerReader;
        }
        // Forgot Pasword
        public async Task<bool> ForgotPassword(string email)
        {
            bool found = false;
            if(email == null || email.IsNullOrEmpty()) return false;
            try
            {
                var reader = await readerRepository.getReaderByEmail(email.Trim());
                var employee = await employeeRepository.GetByEmail(email.Trim());

                if (employee != null)
                {
                    var randomPassword = RandomPassword();
                    var hashedPassword = PasswordHelper.HashPassword(randomPassword);

                    employee.Password = hashedPassword;
                    await employeeRepository.UpdateEmployee(employee);

                    NotificationMessage notificationMessage = new NotificationMessage
                    {
                        ToEmail = email,
                        Subject = "Cập nhật mật khẩu",
                        Body = $"Đây là mật khẩu mới của bạn: {randomPassword}. Hãy đăng nhập và thay đổi mật khẩu.",
                    };
                    await notificationService.NotifyAsync(notificationMessage);

                    found = true;
                }

                if (reader != null)
                {
                    var randomPassword = RandomPassword();
                    var hashedPassword = PasswordHelper.HashPassword(randomPassword);

                    reader.Password = hashedPassword;
                    await readerRepository.UpdateReader(reader);

                    NotificationMessage notificationMessage = new NotificationMessage
                    {
                        ToEmail = email,
                        Subject = "Cập nhật mật khẩu",
                        Body = $"Đây là mật khẩu mới của bạn: {randomPassword}. Hãy đăng nhập và thay đổi mật khẩu.",
                    };
                    await notificationService.NotifyAsync(notificationMessage);

                    found = true;
                }
            }
            catch (Exception ex)
            {
                
                Console.WriteLine($"ForgotPassword error: {ex.Message}");
                return false;
            }

            return found;
        }

        public string RandomPassword()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*";
            var bytes = new byte[12];
            RandomNumberGenerator.Fill(bytes);
            return new string(bytes.Select(b => chars[b % chars.Length]).ToArray());
        }

        // ✅ THÊM MỚI: Method kiểm tra user có tồn tại không (để phân biệt email sai vs password sai)
        public async Task<bool> IsUserExist(string email)
        {
            var reader = await readerRepository.getReaderByEmail(email);
            var employee = await employeeRepository.GetByEmail(email);
            
            return reader != null || employee != null;
        }
    }
}
