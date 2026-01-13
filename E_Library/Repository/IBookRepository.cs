using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_Library.Models;

namespace E_Library.Repository
{
    public interface IBookRepository
    {
       Task<List<Book>> GetAll(string? title, int? publisher ,
                         int? category, string? author, string? language);

        // ✅ THÊM MỚI: Method phân trang
        Task<(List<Book> books, int totalCount)> GetAllPaginated(
            string? title, int? publisher, int? category, 
            string? author, string? language, int page, int pageSize);

        Task<List<Book>> GetAllBook();
       Task<Book> GetById(int ?id);
       Task<Book> CreateBook(Book book);
        Task Save();
        Task Delete(int? bookId);
        Task UpdateBook(Book book);
    }
}
