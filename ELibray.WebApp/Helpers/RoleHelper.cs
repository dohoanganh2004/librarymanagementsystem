using Microsoft.AspNetCore.Http;
using E_Library.Models;
using Microsoft.EntityFrameworkCore;

namespace ELibrary.WebApp.Helpers
{
    public static class RoleHelper
    {
        public static class RoleNames
        {
            public const string Admin = "Admin";
            public const string Librarian = "Librarian";
            public const string Employee = "Employee";
        }

        public static async Task<string?> GetUserRoleAsync(HttpContext context, ElibraryContext dbContext)
        {
            var employeeId = context.Session.GetInt32("employeeId");
            if (!employeeId.HasValue) return null;

            var employee = await dbContext.Employees
                .Include(e => e.Role)
                .FirstOrDefaultAsync(e => e.EmployeeId == employeeId.Value);

            return employee?.Role?.RoleName;
        }

        public static bool IsAdmin(string? roleName)
        {
            return roleName == RoleNames.Admin;
        }

        public static bool IsLibrarian(string? roleName)
        {
            return roleName == RoleNames.Librarian;
        }

        public static bool IsEmployee(string? roleName)
        {
            return roleName == RoleNames.Employee;
        }

        public static bool CanManageBooks(string? roleName)
        {
            return IsAdmin(roleName) || IsLibrarian(roleName);
        }

        public static bool CanManageReservations(string? roleName)
        {
            return IsAdmin(roleName) || IsLibrarian(roleName);
        }

        public static bool CanManageCheckouts(string? roleName)
        {
            return IsAdmin(roleName) || IsLibrarian(roleName);
        }

        public static bool CanManageReaders(string? roleName)
        {
            return IsAdmin(roleName) || IsLibrarian(roleName);
        }

        public static bool CanManageEmployees(string? roleName)
        {
            return IsAdmin(roleName);
        }

        public static bool CanViewStatistics(string? roleName)
        {
            return IsAdmin(roleName) || IsLibrarian(roleName);
        }

        public static bool CanExportData(string? roleName)
        {
            return IsAdmin(roleName) || IsLibrarian(roleName);
        }
    }
}