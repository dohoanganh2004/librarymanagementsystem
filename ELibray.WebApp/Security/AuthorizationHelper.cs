using ELibrary.WebApp.Service;

namespace ELibrary.WebApp.Security
{
    /// <summary>
    /// Helper class để kiểm tra quyền trong Views và Controllers
    /// </summary>
    public static class AuthorizationHelper
    {
        /// <summary>
        /// Kiểm tra user hiện tại có quyền truy cập URL không
        /// </summary>
        public static async Task<bool> HasPermissionAsync(HttpContext httpContext, string url)
        {
            var roleId = httpContext.Session.GetInt32("roleId");
            if (!roleId.HasValue) return false;

            var permissionService = httpContext.RequestServices.GetRequiredService<IPermissionService>();
            return await permissionService.HasPermission(roleId.Value, url.ToLowerInvariant());
        }

        /// <summary>
        /// Kiểm tra có phải Employee không
        /// </summary>
        public static bool IsEmployee(HttpContext httpContext)
        {
            return httpContext.Session.GetInt32("employeeId").HasValue;
        }

        /// <summary>
        /// Kiểm tra có phải Reader không
        /// </summary>
        public static bool IsReader(HttpContext httpContext)
        {
            return httpContext.Session.GetInt32("readerId").HasValue;
        }

        /// <summary>
        /// Lấy RoleId hiện tại
        /// </summary>
        public static int? GetCurrentRoleId(HttpContext httpContext)
        {
            return httpContext.Session.GetInt32("roleId");
        }

        /// <summary>
        /// Lấy EmployeeId hiện tại
        /// </summary>
        public static int? GetCurrentEmployeeId(HttpContext httpContext)
        {
            return httpContext.Session.GetInt32("employeeId");
        }

        /// <summary>
        /// Lấy ReaderId hiện tại
        /// </summary>
        public static int? GetCurrentReaderId(HttpContext httpContext)
        {
            return httpContext.Session.GetInt32("readerId");
        }
    }
}