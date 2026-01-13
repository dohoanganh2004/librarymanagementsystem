using ELibrary.WebApp.DTO.Request;
using ELibrary.WebApp.DTO.Response;
using ELibrary.WebApp.Service;
using ELibrary.WebApp.FileServices;
using Microsoft.AspNetCore.Mvc;

namespace ELibrary.WebApp.Controllers
{
    public class ProfileController : Controller
    {
        private readonly IReaderService _readerService;
        private readonly IEmployeeService _employeeService;
        private readonly IRoleService _roleService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<ProfileController> _logger;

        public ProfileController(
            IReaderService readerService, 
            IEmployeeService employeeService,
            IRoleService roleService,
            IWebHostEnvironment webHostEnvironment,
            ILogger<ProfileController> logger)
        {
            _readerService = readerService;
            _employeeService = employeeService;
            _roleService = roleService;
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;
        }

        public async Task<IActionResult> ReaderProfile()
        {
            var readerId = HttpContext.Session.GetInt32("readerId");

            if (readerId == null || readerId == 0)
            {
                return RedirectToAction("Login", "Auth");
            }

            try
            {
                var reader = await _readerService.getReaderByID(readerId.Value);
                if (reader == null)
                {
                    HttpContext.Session.Clear();
                    return RedirectToAction("Login", "Auth");
                }
                return View(reader);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Lỗi khi tải thông tin hồ sơ: " + ex.Message;
                return View("Error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> EditProfile()
        {
            var readerId = HttpContext.Session.GetInt32("readerId");

            if (readerId == null || readerId == 0)
            {
                return RedirectToAction("Login", "Auth");
            }

            try
            {
                var reader = await _readerService.getReaderByID(readerId.Value);
                if (reader == null)
                {
                    HttpContext.Session.Clear();
                    return RedirectToAction("Login", "Auth");
                }
                return View(reader);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Lỗi khi tải form chỉnh sửa: " + ex.Message;
                return RedirectToAction("ReaderProfile");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(ReaderUpdateRequestDTO readerUpdateRequestDTO)
        {
            var readerId = HttpContext.Session.GetInt32("readerId");

            if (readerId == null || readerId == 0 || readerId != readerUpdateRequestDTO.ReaderId)
            {
                return RedirectToAction("Login", "Auth");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _readerService.UpdateReader(readerUpdateRequestDTO);

                    TempData["SuccessMessage"] = "Thông tin cá nhân đã được cập nhật thành công.";
                    return RedirectToAction("ReaderProfile");
                }
                catch (Exception ex)
                {
                    ViewBag.ErrorMessage = "Cập nhật thất bại: " + ex.Message;

                    return await GetModelAndReturnView(readerId.Value, readerUpdateRequestDTO, "EditProfile");
                }
            }

            ViewBag.ErrorMessage = "Dữ liệu nhập không hợp lệ. Vui lòng kiểm tra lại.";
            return await GetModelAndReturnView(readerId.Value, readerUpdateRequestDTO, "EditProfile");
        }

        private async Task<IActionResult> GetModelAndReturnView(int readerId, ReaderUpdateRequestDTO updateDTO, string viewName)
        {
            var readerResponse = await _readerService.getReaderByID(readerId);

            if (readerResponse != null)
            {
                readerResponse.FirstName = updateDTO.FirstName;
                readerResponse.LastName = updateDTO.LastName;
                readerResponse.PhoneNumber = updateDTO.PhoneNumber;
                readerResponse.Address = updateDTO.Address;
                readerResponse.DoB = updateDTO.DoB;
            }

            return View(viewName, readerResponse);
        }

        // ================== EMPLOYEE PROFILE METHODS ==================
        
        [HttpGet]
        public async Task<IActionResult> EmployeeProfile()
        {
            var employeeId = HttpContext.Session.GetInt32("employeeId");

            if (employeeId == null || employeeId == 0)
            {
                return RedirectToAction("Login", "Auth");
            }

            try
            {
                var employee = await _employeeService.GetByID(employeeId.Value);
                if (employee == null)
                {
                    HttpContext.Session.Clear();
                    return RedirectToAction("Login", "Auth");
                }
                return View(employee);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading employee profile for ID: {EmployeeId}", employeeId);
                ViewBag.ErrorMessage = "Lỗi khi tải thông tin hồ sơ: " + ex.Message;
                return View("Error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> EditEmployeeProfile()
        {
            var employeeId = HttpContext.Session.GetInt32("employeeId");

            if (employeeId == null || employeeId == 0)
            {
                return RedirectToAction("Login", "Auth");
            }

            try
            {
                var employee = await _employeeService.GetByID(employeeId.Value);
                if (employee == null)
                {
                    HttpContext.Session.Clear();
                    return RedirectToAction("Login", "Auth");
                }

                // Load roles for dropdown
                ViewBag.Roles = await _roleService.GetAll();

                // Convert to EmployeeRequestDTO for editing
                var employeeRequest = new EmployeeRequestDTO
                {
                    EmployeeId = employee.EmployeeId,
                    FirstName = employee.FirstName,
                    LastName = employee.LastName,
                    Email = employee.Email,
                    PhoneNumber = employee.PhoneNumber,
                    DoB = employee.DoB,
                    Age = employee.Age,
                    Avatar = employee.Avatar,
                    RoleId = employee.RoleId,
                    StartDate = employee.StartDate,
                    Status = employee.Status
                };

                return View(employeeRequest);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading employee edit form for ID: {EmployeeId}", employeeId);
                ViewBag.ErrorMessage = "Lỗi khi tải form chỉnh sửa: " + ex.Message;
                return RedirectToAction("EmployeeProfile");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditEmployeeProfile(EmployeeRequestDTO employeeRequestDTO)
        {
            var employeeId = HttpContext.Session.GetInt32("employeeId");

            if (employeeId == null || employeeId == 0 || employeeId != employeeRequestDTO.EmployeeId)
            {
                return RedirectToAction("Login", "Auth");
            }

            try
            {
                // Handle avatar upload
                if (employeeRequestDTO.AvatarFile != null && employeeRequestDTO.AvatarFile.Length > 0)
                {
                    _logger.LogInformation("Processing avatar upload for employee {EmployeeId}", employeeId);
                    
                    // Validate file
                    var allowedTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/gif" };
                    if (!allowedTypes.Contains(employeeRequestDTO.AvatarFile.ContentType.ToLower()))
                    {
                        ModelState.AddModelError("AvatarFile", "Chỉ chấp nhận file ảnh (JPG, PNG, GIF)");
                    }
                    else if (employeeRequestDTO.AvatarFile.Length > 5 * 1024 * 1024)
                    {
                        ModelState.AddModelError("AvatarFile", "File ảnh không được vượt quá 5MB");
                    }
                    else
                    {
                        try
                        {
                            employeeRequestDTO.Avatar = await FileHelper.UploadFileAsync(
                                employeeRequestDTO.AvatarFile,
                                "img/employees",
                                _webHostEnvironment.WebRootPath
                            );
                            _logger.LogInformation("Avatar uploaded successfully: {AvatarPath}", employeeRequestDTO.Avatar);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error uploading avatar for employee {EmployeeId}", employeeId);
                            ModelState.AddModelError("AvatarFile", "Lỗi khi upload ảnh: " + ex.Message);
                        }
                    }
                }

                if (ModelState.IsValid)
                {
                    await _employeeService.Update(employeeRequestDTO);
                    _logger.LogInformation("Employee profile updated successfully for ID: {EmployeeId}", employeeId);

                    TempData["SuccessMessage"] = "Thông tin hồ sơ đã được cập nhật thành công.";
                    return RedirectToAction("EmployeeProfile");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating employee profile for ID: {EmployeeId}", employeeId);
                ModelState.AddModelError("", "Cập nhật thất bại: " + ex.Message);
            }

            // Reload roles for dropdown if validation fails
            ViewBag.Roles = await _roleService.GetAll();
            return View(employeeRequestDTO);
        }
    }
}
