using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using ELibrary.WebApp.Security;

namespace ELibrary.WebApp.Extensions
{
    /// <summary>
    /// Extension methods cho HTML Helper để kiểm tra quyền trong Views
    /// </summary>
    public static class HtmlHelperExtensions
    {
        /// <summary>
        /// Kiểm tra user có quyền truy cập URL không
        /// Sử dụng: @if(Html.HasPermission("/Book/Create")) { ... }
        /// </summary>
        public static async Task<bool> HasPermissionAsync(this IHtmlHelper htmlHelper, string url)
        {
            var httpContext = htmlHelper.ViewContext.HttpContext;
            return await AuthorizationHelper.HasPermissionAsync(httpContext, url);
        }

        /// <summary>
        /// Render HTML nếu có quyền
        /// Sử dụng: @Html.IfHasPermission("/Book/Create", "<a href='/Book/Create'>Tạo sách</a>")
        /// </summary>
        public static async Task<IHtmlContent> IfHasPermissionAsync(this IHtmlHelper htmlHelper, string url, string htmlContent)
        {
            var hasPermission = await htmlHelper.HasPermissionAsync(url);
            return hasPermission ? new HtmlString(htmlContent) : HtmlString.Empty;
        }

        /// <summary>
        /// Kiểm tra có phải Employee không
        /// </summary>
        public static bool IsEmployee(this IHtmlHelper htmlHelper)
        {
            var httpContext = htmlHelper.ViewContext.HttpContext;
            return AuthorizationHelper.IsEmployee(httpContext);
        }

        /// <summary>
        /// Kiểm tra có phải Reader không
        /// </summary>
        public static bool IsReader(this IHtmlHelper htmlHelper)
        {
            var httpContext = htmlHelper.ViewContext.HttpContext;
            return AuthorizationHelper.IsReader(httpContext);
        }

        /// <summary>
        /// Lấy RoleId hiện tại
        /// </summary>
        public static int? GetCurrentRoleId(this IHtmlHelper htmlHelper)
        {
            var httpContext = htmlHelper.ViewContext.HttpContext;
            return AuthorizationHelper.GetCurrentRoleId(httpContext);
        }
    }
}