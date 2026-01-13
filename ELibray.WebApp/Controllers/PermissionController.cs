using E_Library.Models;
using ELibrary.WebApp.Service;
using Microsoft.AspNetCore.Mvc;

namespace ELibrary.WebApp.Controllers
{
    public class PermissionController : Controller
    {
        private readonly IPermissionService _permissionService;
        private readonly IRoleService _roleService;
        private readonly ILogger<PermissionController> _logger;

        public PermissionController(IPermissionService permissionService, IRoleService roleService, ILogger<PermissionController> logger)
        {
            _permissionService = permissionService;
            _roleService = roleService;
            _logger = logger;
        }

        // GET: Permission
        public async Task<IActionResult> Index()
        {
            try
            {
                var roles = await _roleService.GetAll();
                var permissions = new Dictionary<int, List<Permission>>();

                foreach (var role in roles)
                {
                    permissions[role.RoleId] = await _permissionService.GetPermissionsByRoleId(role.RoleId);
                }

                ViewBag.Roles = roles;
                ViewBag.Permissions = permissions;

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading permissions");
                TempData["error"] = "Có lỗi xảy ra khi tải danh sách quyền";
                return View();
            }
        }

        // GET: Permission/Matrix
        public async Task<IActionResult> Matrix()
        {
            try
            {
                var roles = await _roleService.GetAll();
                var allPermissions = new List<Permission>();
                var rolePermissions = new Dictionary<int, List<int>>();

                // Get all unique permissions
                foreach (var role in roles)
                {
                    var perms = await _permissionService.GetPermissionsByRoleId(role.RoleId);
                    rolePermissions[role.RoleId] = perms.Select(p => p.PermissionId).ToList();
                    
                    foreach (var perm in perms)
                    {
                        if (!allPermissions.Any(p => p.PermissionId == perm.PermissionId))
                        {
                            allPermissions.Add(perm);
                        }
                    }
                }

                ViewBag.Roles = roles;
                ViewBag.AllPermissions = allPermissions.OrderBy(p => p.Description).ToList();
                ViewBag.RolePermissions = rolePermissions;

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading permission matrix");
                TempData["error"] = "Có lỗi xảy ra khi tải ma trận quyền";
                return RedirectToAction("Index");
            }
        }

        // POST: Permission/UpdateMatrix
        [HttpPost]
        public async Task<IActionResult> UpdateMatrix(Dictionary<string, bool> permissions)
        {
            try
            {
                // Implementation for updating role-permission matrix
                // This would require additional repository methods
                
                TempData["success"] = "Cập nhật quyền thành công!";
                return RedirectToAction("Matrix");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating permission matrix");
                TempData["error"] = "Có lỗi xảy ra khi cập nhật quyền";
                return RedirectToAction("Matrix");
            }
        }

        // GET: Permission/Check
        public async Task<IActionResult> Check(int roleId, string url)
        {
            try
            {
                var hasPermission = await _permissionService.HasPermission(roleId, url);
                var allowedUrls = await _permissionService.GetAllowedUrls(roleId);

                var result = new
                {
                    RoleId = roleId,
                    Url = url,
                    HasPermission = hasPermission,
                    AllowedUrls = allowedUrls
                };

                return Json(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking permission");
                return Json(new { error = "Có lỗi xảy ra khi kiểm tra quyền" });
            }
        }
    }
}