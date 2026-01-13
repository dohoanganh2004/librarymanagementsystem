using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http; // Cần thiết cho IFormFile

namespace ELibrary.WebApp.DTO.Request
{
    public class EmployeeRequestDTO
    {
        public int EmployeeId { get; set; }

      

        [Required(ErrorMessage = "Tên (First Name) là bắt buộc.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Tên phải dài từ 2 đến 50 ký tự.")]
        public string FirstName { get; set; } = null!;

        [Required(ErrorMessage = "Họ (Last Name) là bắt buộc.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Họ phải dài từ 2 đến 50 ký tự.")]
        public string LastName { get; set; } = null!;

      

        [Required(ErrorMessage = "Ngày sinh (DoB) là bắt buộc.")]
        
        public DateOnly? DoB { get; set; }

        
        [Range(18, 65, ErrorMessage = "Tuổi phải từ 18 đến 65.")]
        public int? Age { get; set; }

        

        [Required(ErrorMessage = "Email là bắt buộc.")]
        [EmailAddress(ErrorMessage = "Định dạng Email không hợp lệ.")]
        [StringLength(100, ErrorMessage = "Email không được vượt quá 100 ký tự.")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Số điện thoại là bắt buộc.")]
        [Phone(ErrorMessage = "Định dạng số điện thoại không hợp lệ.")]
        [StringLength(15, ErrorMessage = "Số điện thoại không được vượt quá 15 ký tự.")]
        public string? PhoneNumber { get; set; }

        
        [StringLength(16, MinimumLength = 8, ErrorMessage = "Mật khẩu phải dài từ 8 đến 16 ký tự.")]
        public string? Password { get; set; } = null!;

        

        
        public IFormFile? AvatarFile { get; set; }

        [StringLength(500, ErrorMessage = "Đường dẫn Avatar không được vượt quá 500 ký tự.")]
        [DataType(DataType.ImageUrl)]
        public string? Avatar { get; set; }

      

        [Required(ErrorMessage = "Vai trò (Role ID) là bắt buộc.")]
        [Range(1, int.MaxValue, ErrorMessage = "Role ID phải là số nguyên dương.")]
        public int RoleId { get; set; }

        [Required(ErrorMessage = "Ngày bắt đầu làm việc (Start Date) là bắt buộc.")]
        
        public DateOnly StartDate { get; set; }

        public bool? Status { get; set; }
    }
}