using ELibrary.WebApp.DTO.Request;
using ELibrary.WebApp.Service;
using Microsoft.AspNetCore.Mvc;

namespace ELibrary.WebApp.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly IEmployeeService _employeeService;
        private readonly IRoleService _roleService;

        public EmployeeController(IEmployeeService employeeService, IRoleService roleService)
        {
            _employeeService = employeeService;
            _roleService = roleService;
        }

        //-----------------------  All employee --------------------
        public async Task<IActionResult> All(string? firstName, string? lastName, DateOnly? Dob,
            string? email, string? phoneNumber, int? RoleID, bool? status)
        {
            var employees = await _employeeService.GetAll(firstName, lastName, Dob, email, phoneNumber, RoleID, status);
            return View(employees);
        }

        //----------------------- Details ---------------------------
        public async Task<IActionResult> Details(int? id)
        {
            var employee = await _employeeService.GetByID(id);
            return View(employee);
        }

        // --------------------- Delete ------------------------------
        public async Task<IActionResult> Delete(int? id)
        {
            await _employeeService.Delete(id);
            return RedirectToAction("All");
        }

        // --------------------- Create Employee ----------------------
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var roles = await _roleService.GetAll();
            ViewBag.Roles = roles;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(EmployeeRequestDTO employeeRequestDTO)
        {
            if (!ModelState.IsValid)
            {
                var roles = await _roleService.GetAll();
                ViewBag.Roles = roles;
                return View(employeeRequestDTO);
            }

            try
            {
                await _employeeService.Create(employeeRequestDTO);

                TempData["SuccessMessage"] = "Nhân viên đã được tạo thành công và email đã được gửi!";
                return RedirectToAction("All");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);

                var roles = await _roleService.GetAll();
                ViewBag.Roles = roles;

                return View(employeeRequestDTO);
            }
        }

        //------------------------ Update Employee ------------------------
        [HttpGet]
        public async Task<IActionResult> Update(int? id)
        {
            var employee = await _employeeService.GetByID(id);
            var roles = await _roleService.GetAll();
            ViewBag.Roles = roles;

            return View(employee);
        }

        [HttpPost]
        public async Task<IActionResult> Update(EmployeeRequestDTO employeeRequestDTO)
        {
            if (!ModelState.IsValid)
            {
                var roles = await _roleService.GetAll();
                ViewBag.Roles = roles;
                return View(employeeRequestDTO);
            }

            await _employeeService.Update(employeeRequestDTO);
            return RedirectToAction("All");
        }


        // change status of account employee
        public async Task<IActionResult> ChangeStatus(int ? id , string? actionType)
        {
            bool ok = await _employeeService.ChangeStatus(id,actionType);

            TempData["msg"] = ok
                ? $"Action successfully!"
                : "Action failed!";

            return RedirectToAction("All");
        }
    }
}
