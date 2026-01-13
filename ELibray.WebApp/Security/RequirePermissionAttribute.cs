using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ELibrary.WebApp.Service;

namespace ELibrary.WebApp.Security
{
    /// <summary>
    /// Attribute để kiểm tra quyền truy cập cho Controller/Action
    /// Sử dụng: [RequirePermission("/Book/Create")]
    /// </summary>
    public class RequirePermissionAttribute : ActionFilterAttribute
    {
        private readonly string _requiredUrl;

        public RequirePermissionAttribute(string requiredUrl)
        {
            _requiredUrl = requiredUrl.ToLowerInvariant();
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var httpContext = context.HttpContext;
            
            // Kiểm tra đăng nhập
            var employeeId = httpContext.Session.GetInt32("employeeId");
            var roleId = httpContext.Session.GetInt32("roleId");

            if (!employeeId.HasValue || !roleId.HasValue)
            {
                context.Result = new RedirectResult("/Auth/Login");
                return;
            }

            // Kiểm tra quyền
            var permissionService = httpContext.RequestServices.GetRequiredService<IPermissionService>();
            bool hasPermission = await permissionService.HasPermission(roleId.Value, _requiredUrl);

            if (!hasPermission)
            {
                context.Result = new ContentResult
                {
                    StatusCode = 403,
                    Content = "Bạn không có quyền thực hiện hành động này.",
                    ContentType = "text/plain; charset=utf-8"
                };
                return;
            }

            await next();
        }
    }
}