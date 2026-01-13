using E_Library.Models;
using ELibrary.WebApp.DTO.Request;
using ELibrary.WebApp.DTO.Response;
using ELibrary.WebApp.Models;

namespace ELibrary.WebApp.Service
{
    public interface IBookService
    {
        Task<List<BookResponseDTO>> GetAll(string? title,  int? publisher,
                       int? category, string? author, string? language);
        
        // ✅ THÊM MỚI: Method phân trang
        Task<PaginationModel<BookResponseDTO>> GetAllPaginated(BookSearchParameters parameters);
        
        Task<BookResponseDTO> GetBook(int ?id);
        Task<Book> GetBookByID(int? id);
        Task<BookResponseDTO> UpdateBook(BookRequestDTO bookRequestDTO);
        Task DeleteBook(int? id);
        Task<BookResponseDTO> CreateBook(BookRequestDTO bookRequestDTO);
    }
}
