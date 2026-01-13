using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using ELibrary.WebApp.Service;
using E_Library.Models;
using ELibrary.WebApp.DTO.Request;
using System.Text.Json;

namespace ELibrary.WebApp.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService authService;
        private readonly IPermissionService permissionService;
        private readonly IAuditLogService _auditLogService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, IPermissionService permissionService, IAuditLogService auditLogService, ILogger<AuthController> logger)
        {
            this.authService = authService;
            this.permissionService = permissionService;
            _auditLogService = auditLogService;
            _logger = logger;
        }

        // GET: /Auth/Login
        [HttpGet]
        public IActionResult Login()
        {
            if (HttpContext.Session.GetInt32("readerId").HasValue)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        // POST: /Auth/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string email, string password)
        {
            const int MAX_FAIL_ATTEMPTS = 5;
            const int LOCKOUT_DURATION_SECONDS = 30;

           
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ViewBag.ErrorMessage = "Vui lòng nhập đầy đủ email và mật khẩu.";
                return View();
            }

            
            var lockTimeSession = HttpContext.Session.GetString("lockTime");
            if (!string.IsNullOrEmpty(lockTimeSession) &&
                DateTime.TryParse(lockTimeSession, out DateTime lockTime) &&
                lockTime > DateTime.Now)
            {
                var remainingSeconds = (int)(lockTime - DateTime.Now).TotalSeconds;
                ViewBag.ErrorMessage = $"Tài khoản đã bị khóa do đăng nhập sai quá {MAX_FAIL_ATTEMPTS} lần. Vui lòng thử lại sau {remainingSeconds} giây.";
                return View();
            }

           
            if (!string.IsNullOrEmpty(lockTimeSession))
            {
                HttpContext.Session.Remove("lockTime");
                HttpContext.Session.SetInt32("loginFailCount", 0); 
            }

            try
            {
                
                var reader = await authService.LoginReader(email, password);
                if (reader != null)
                {
                    
                    HttpContext.Session.SetInt32("readerId", reader.ReaderId);
                    HttpContext.Session.SetString("readerName",reader.FirstName);
                    HttpContext.Session.SetInt32("loginFailCount", 0);
                    HttpContext.Session.Remove("lockTime");

                    _logger.LogInformation($"Reader login successful: {reader.FirstName} {reader.LastName} (ID: {reader.ReaderId})");
                    
                    return RedirectToAction("Index", "Home");
                }

            
                var employee = await authService.LoginEmployee(email, password);
                if (employee != null)
                {
                  
                    HttpContext.Session.SetInt32("employeeId", employee.EmployeeId);
                    HttpContext.Session.SetInt32("roleId", employee.RoleId);
                    HttpContext.Session.SetString("employeeName", employee.FirstName);
                    HttpContext.Session.SetInt32("loginFailCount", 0);
                    HttpContext.Session.Remove("lockTime");
               
                    var permissions = await permissionService.GetPermissionLinksForSession(employee.Role.RoleId);
                    HttpContext.Session.SetString("userPermissions", string.Join(",", permissions));

                    // Log audit trail for employee login
                    await _auditLogService.LogActionAsync(
                        employee.EmployeeId,
                        $"Đăng nhập hệ thống",
                        "Auth",
                        null,
                        null,
                        JsonSerializer.Serialize(new { 
                            LoginTime = DateTime.Now,
                            Email = email,
                            Role = employee.Role?.RoleName,
                            IPAddress = HttpContext.Connection.RemoteIpAddress?.ToString()
                        })
                    );

                    _logger.LogInformation($"Employee login successful: {employee.FirstName} {employee.LastName} (ID: {employee.EmployeeId}, Role: {employee.Role?.RoleName})");
                    
                    return RedirectToAction("Index", "Home");
                }

                
                var failCount = HttpContext.Session.GetInt32("loginFailCount") ?? 0;
                failCount++;
                HttpContext.Session.SetInt32("loginFailCount", failCount);

                if (failCount >= MAX_FAIL_ATTEMPTS)
                {
                    
                    var newLockTime = DateTime.Now.AddSeconds(LOCKOUT_DURATION_SECONDS);
                    HttpContext.Session.SetString("lockTime", newLockTime.ToString());
                    
                    ViewBag.ErrorMessage = $"Đăng nhập sai quá {MAX_FAIL_ATTEMPTS} lần. Tài khoản tạm thời bị khóa trong {LOCKOUT_DURATION_SECONDS} giây.";
                }
                else
                {
                   
                    ViewBag.ErrorMessage = $"Email hoặc mật khẩu không hợp lệ. Bạn còn {MAX_FAIL_ATTEMPTS - failCount} lần thử.";
                }

                return View();
            }
            catch (Exception ex)
            {
                
                ViewBag.ErrorMessage = "Có lỗi xảy ra trong quá trình đăng nhập. Vui lòng thử lại.";
                return View();
            }
        }


        // GET: /Auth/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Auth/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(ReaderRequestDTO readerDto)
        {
            if (!ModelState.IsValid)
            {
                return View(readerDto);
            }

            try
            {
                var registerReader = await authService.Register(readerDto);

                if (registerReader != null)
                {
                    TempData["SuccessMessage"] = "Registration successful! You can now sign in.";
                    return RedirectToAction("Login", "Auth");
                }

                ModelState.AddModelError("", "Registration failed. Please try again.");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }

            return View(readerDto);
        }


        // POST: /Auth/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            // Log audit trail before clearing session
            var employeeId = HttpContext.Session.GetInt32("employeeId");
            var employeeName = HttpContext.Session.GetString("employeeName");
            var readerId = HttpContext.Session.GetInt32("readerId");
            var readerName = HttpContext.Session.GetString("readerName");

            if (employeeId.HasValue)
            {
                await _auditLogService.LogActionAsync(
                    employeeId.Value,
                    $"Đăng xuất khỏi hệ thống",
                    "Auth",
                    null,
                    null,
                    JsonSerializer.Serialize(new { 
                        LogoutTime = DateTime.Now,
                        IPAddress = HttpContext.Connection.RemoteIpAddress?.ToString()
                    })
                );

                _logger.LogInformation($"Employee logout: {employeeName} (ID: {employeeId})");
            }
            else if (readerId.HasValue)
            {
                _logger.LogInformation($"Reader logout: {readerName} (ID: {readerId})");
            }

            HttpContext.Session.Clear();
            return RedirectToAction("Index","Home");
        }


        //----------------------- forgot password -----------------
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            bool result = await authService.ForgotPassword(email);

            if (result)
            {
                ViewBag.Message = "Vui lòng kiểm tra email để lấy mật khẩu mới.";
            }
            else
            {
                ViewBag.Message = "Email không tồn tại hoặc có lỗi xảy ra.";
            }

            return View();
        }

        //----------------------- change password -----------------
        [HttpGet]
        public IActionResult ChangePassword()
        {
           
            var readerId = HttpContext.Session.GetInt32("readerId");
            var employeeId = HttpContext.Session.GetInt32("employeeId");
            
            if (!readerId.HasValue && !employeeId.HasValue)
            {
                return RedirectToAction("Login");
            }

            var model = new ChangePasswordRequestDTO
            {
                ReaderId = readerId,
                EmployeeId = employeeId
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordRequestDTO model)
        {
            try
            {
                // Check if user is logged in
                var readerId = HttpContext.Session.GetInt32("readerId");
                var employeeId = HttpContext.Session.GetInt32("employeeId");
                
                if (!readerId.HasValue && !employeeId.HasValue)
                {
                    return RedirectToAction("Login");
                }

                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                
                model.ReaderId = readerId;
                model.EmployeeId = employeeId;

                
                var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

                bool result = false;
                
               
                var passwordService = HttpContext.RequestServices.GetRequiredService<IPasswordService>();

                if (readerId.HasValue)
                {
                    result = await passwordService.ChangeReaderPasswordAsync(readerId.Value, model);
                    if (result)
                    {
                        await passwordService.LogPasswordChangeAsync(readerId.Value, "reader", ipAddress);
                    }
                }
                else if (employeeId.HasValue)
                {
                    result = await passwordService.ChangeEmployeePasswordAsync(employeeId.Value, model);
                    if (result)
                    {
                        await passwordService.LogPasswordChangeAsync(employeeId.Value, "employee", ipAddress);
                    }
                }

                if (result)
                {
                    TempData["SuccessMessage"] = "Đổi mật khẩu thành công!";
                    return RedirectToAction("ChangePassword");
                }
                else
                {
                    ModelState.AddModelError("CurrentPassword", "Mật khẩu hiện tại không đúng hoặc có lỗi xảy ra.");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Có lỗi xảy ra khi đổi mật khẩu. Vui lòng thử lại.");
            }

            return View(model);
        }

    }
}
