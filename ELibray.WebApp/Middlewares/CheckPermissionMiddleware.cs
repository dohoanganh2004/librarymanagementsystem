using ELibrary.WebApp.Service;

namespace ELibrary.WebApp.Middlewares
{
    public class CheckPermissionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public CheckPermissionMiddleware(RequestDelegate next, IServiceScopeFactory serviceScopeFactory)
        {
            _next = next;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value?.ToLowerInvariant();

            // Các đường dẫn không cần kiểm tra quyền
            bool isExcludedPath =
                string.IsNullOrEmpty(path) ||
                path == "/" ||
                path.StartsWith("/auth/") ||
                path.StartsWith("/home/") ||
                path.StartsWith("/assets") ||
                path.StartsWith("/css") ||
                path.StartsWith("/js") ||
                path.StartsWith("/lib") ||
                path.StartsWith("/img") ||
                path.StartsWith("/favicon.ico");

            if (isExcludedPath)
            {
                await _next(context);
                return;
            }


            var readerId = context.Session.GetInt32("readerId");
            var employeeId = context.Session.GetInt32("employeeId");
            var roleId = context.Session.GetInt32("roleId");


            if (readerId.HasValue)
            {

                bool isReaderAllowed =
                    path.StartsWith("/book/all") ||
                    path.StartsWith("/book/detail") ||
                    path.StartsWith("/profile/") ||
                    path.StartsWith("/reservation/") ||
                    path.StartsWith("/checkout/");

                if (isReaderAllowed)
                {
                    await _next(context);
                    return;
                }
                else
                {

                    context.Response.StatusCode = 403;
                    await context.Response.WriteAsync("Bạn không có quyền truy cập trang này.");
                    return;
                }
            }


            if (employeeId.HasValue && roleId.HasValue)
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var permissionService = scope.ServiceProvider.GetRequiredService<IPermissionService>();


                bool hasPermission = await permissionService.HasPermission(roleId.Value, path);

                if (hasPermission)
                {
                    await _next(context);
                    return;
                }
                else
                {

                    context.Response.StatusCode = 403;
                    await context.Response.WriteAsync("Bạn không có quyền truy cập trang này.");
                    return;
                }
            }


            context.Response.Redirect("/Auth/Login");
        }
    }
}
