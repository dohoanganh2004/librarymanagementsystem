namespace ELibrary.WebApp.Middlewares
{
    public class CheckLoginMiddleware
    {
        private readonly RequestDelegate _next;

        public CheckLoginMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value?.ToLowerInvariant(); 
            bool isExcludedPath =
                string.IsNullOrEmpty(path) ||
                path == "/" ||
                path.StartsWith("/home") ||
                path.StartsWith("/book/all") ||
                path.StartsWith("/book/detail") ||
                path.StartsWith("/auth/login") ||
                path.StartsWith("/auth/register") ||
                path.StartsWith("/auth/logout") ||
                path.StartsWith("/auth/forgotpassword") ||
                path.StartsWith("/assets") ||
                path.StartsWith("/css") ||
                path.StartsWith("/js") ||
                path.StartsWith("/favicon.ico"); 

            if (isExcludedPath)
            {
                await _next(context);
                return;
            }

          
            var reader = context.Session.GetInt32("readerId");
            var employee = context.Session.GetInt32("employeeId");

          
            if (reader == null && employee == null)
            {
                
                context.Response.Redirect("/Auth/Login");
                return;
            }

            
            await _next(context);
        }
    }
}