using System.ComponentModel.DataAnnotations;
using E_Library.Models;

namespace ELibrary.WebApp.DTO.Request
{
    public class BookRequestDTO
    {
      
        public int BookId { get; set; }


  
        [Required(ErrorMessage = "Tên sách (Title) là bắt buộc.")]
        [StringLength(255, MinimumLength = 3, ErrorMessage = "Tên sách phải dài từ 3 đến 255 ký tự.")]
        public string Title { get; set; } = null!;

        [Required(ErrorMessage = "Năm xuất bản là bắt buộc.")]
        [Range(1800, 2025, ErrorMessage = "Năm xuất bản phải từ 1800 đến năm hiện tại.")]
        public int PublicationYear { get; set; }

        
      

      
      
        public int? PublisherId { get; set; }

       
        public int? CategoryId { get; set; }

       
        public int? AuthorId { get; set; }


      
        [StringLength(500, ErrorMessage = "Đường dẫn hình ảnh không được vượt quá 500 ký tự.")]
        [DataType(DataType.ImageUrl)] 
        public string? Image { get; set; }

        public IFormFile? ImageFile { get; set; }

        [StringLength(2000, ErrorMessage = "Mô tả không được vượt quá 2000 ký tự.")]
        public string? Description { get; set; }


       
        [Required(ErrorMessage = "Số trang (PageCount) là bắt buộc.")]
        [Range(1, 10000, ErrorMessage = "Số trang phải từ 1 đến 10000.")]
        public int PageCount { get; set; }


       
        [StringLength(50, ErrorMessage = "Ngôn ngữ không được vượt quá 50 ký tự.")]
        public string? Language { get; set; }


       
        [Required(ErrorMessage = "Số lượng (Quantity) là bắt buộc.")]
        [Range(0, 10000, ErrorMessage = "Số lượng phải là số nguyên không âm.")]
        public int Quantity { get; set; }
        public bool? Status { get; set; }
    }
}