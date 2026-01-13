using E_Library.Models;

namespace ELibrary.WebApp.DTO.Response
{
    public class EmployeeResponseDTO
    {
        public int EmployeeId { get; set; }

        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public DateOnly? DoB { get; set; }

        public int? Age { get; set; }

        public string Email { get; set; } = null!;

        public string? PhoneNumber { get; set; }

        public string Password { get; set; } = null!;

        public string? Avatar { get; set; }

        public int RoleId { get; set; }

        public string? RoleName { get; set; }

        public DateOnly StartDate { get; set; }

        public bool? Status { get; set; }

      
    }
}
