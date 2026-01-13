using AutoMapper;
using E_Library.Models;
using E_Library.Repository;
using ELibrary.WebApp.DTO.Request;
using ELibrary.WebApp.DTO.Response;
using ELibrary.WebApp.EmailServices;
using ELibrary.WebApp.Models;
using ELibrary.WebApp.Security;
using System.Security.Cryptography;

namespace ELibrary.WebApp.Service
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly INotificationService _notificationService;
        private readonly IMapper mapper;
        private readonly ILogger<EmployeeService> _logger;

        public EmployeeService(
              IEmployeeRepository employeeRepository,
              IMapper mapper,
              INotificationService notificationService,
              ILogger<EmployeeService> logger)
        {
            _employeeRepository = employeeRepository;
            this.mapper = mapper;
            _notificationService = notificationService;
            _logger = logger;
        }

        // ============================ CREATE ================================
        public async Task<EmployeeResponseDTO> Create(EmployeeRequestDTO employeeRequestDTO)
        {
            _logger.LogInformation("Bắt đầu tạo nhân viên với email: {Email}", employeeRequestDTO.Email);

            try
            {
                var employeeByEmail = await _employeeRepository.GetByEmail (employeeRequestDTO.Email);
                if (employeeByEmail != null)
                    throw new Exception("Email này đã tồn tại!");

                var employeeByPhone = await _employeeRepository.GetByPhoneNumber(employeeRequestDTO.PhoneNumber);
                if (employeeByPhone != null)
                    throw new Exception("SĐT này đã tồn tại!");

                // Random password
                var randomPassword = RandomPassword();
                employeeRequestDTO.Password = PasswordHelper.HashPassword(randomPassword);

                var newEmployee = mapper.Map<Employee>(employeeRequestDTO);

                await _employeeRepository.CreateEmployee(newEmployee);

                // Send email
                var notification = new NotificationMessage
                {
                    ToEmail = employeeRequestDTO.Email,
                    Subject = "Tài khoản nhân viên E-Library",
                    Body = $@"
                        <h3>Chào {employeeRequestDTO.FirstName} {employeeRequestDTO.LastName},</h3>
                        <p>Email đăng nhập: <b>{employeeRequestDTO.Email}</b></p>
                        <p>Mật khẩu tạm thời: <b>{randomPassword}</b></p>
                        <p>Hãy đổi mật khẩu sau khi đăng nhập.</p>"
                };

                await _notificationService.NotifyAsync(notification);

                return mapper.Map<EmployeeResponseDTO>(newEmployee);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo nhân viên: {Msg}", ex.Message);
                throw;
            }
        }

        // ============================ DELETE ================================
        public async Task Delete(int? id)
        {
            if (id == null)
                throw new Exception("ID không hợp lệ");

            await _employeeRepository.DeleteEmployee(id.Value);
        }

        // ============================ GET BY ID ================================
        public async Task<EmployeeResponseDTO> GetByID(int? id)
        {
            if (id == null)
                throw new Exception("ID không hợp lệ");

            var employee = await _employeeRepository.GetById(id.Value);

            if (employee == null)
                throw new Exception("Không tìm thấy nhân viên");

            return mapper.Map<EmployeeResponseDTO>(employee);
        }

        // ============================ GET ALL ================================
        public async Task<List<EmployeeResponseDTO>> GetAll(
            string? firstName,
            string? lastName,
            DateOnly? Dob,
            string? email,
            string? phoneNumber,
            int? RoleID,
            bool? status)
        {
            var employees = await _employeeRepository.GetAll(
                firstName, lastName, Dob, email, phoneNumber, RoleID, status
            );

            return mapper.Map<List<EmployeeResponseDTO>>(employees);
        }

        // ============================ UPDATE ================================
        public async Task<EmployeeResponseDTO> Update(EmployeeRequestDTO employeeRequestDTO)
        {
            var existing = await _employeeRepository.GetById(employeeRequestDTO.EmployeeId);

            if (existing == null)
                throw new Exception("Nhân viên không tồn tại!");

            // Nếu không nhập password mới → giữ lại password cũ
            if (string.IsNullOrWhiteSpace(employeeRequestDTO.Password))
                employeeRequestDTO.Password = existing.Password;
            else
                employeeRequestDTO.Password = PasswordHelper.HashPassword(employeeRequestDTO.Password);

            var updated = mapper.Map<Employee>(employeeRequestDTO);

            await _employeeRepository.UpdateEmployee(updated);

            return mapper.Map<EmployeeResponseDTO>(updated);
        }

        // ============================ RANDOM PASSWORD ================================
        public string RandomPassword()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*";
            var bytes = new byte[12];
            RandomNumberGenerator.Fill(bytes);
            return new string(bytes.Select(b => chars[b % chars.Length]).ToArray());
        }
        // change status
        public async Task<bool> ChangeStatus(int? employeeId, string? actionType)
        {
            var employee = await _employeeRepository.GetById(employeeId);
            if(employee == null)
            {
                return false;
            }
            switch (actionType.ToLower())
            {
                case "lock": 
                    employee.Status = false;
                    break;
                case "unlock":
                    employee.Status = true; 
                    break;
                default:
                    return false;
            }
            _employeeRepository.UpdateEmployee(employee);
            return true;
        }
    }
}
