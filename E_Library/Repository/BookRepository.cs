using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_Library.Models;
using Microsoft.EntityFrameworkCore;

namespace E_Library.Repository
{
    public class BookRepository : IBookRepository
    {
        private readonly ElibraryContext _context;

        public BookRepository(ElibraryContext context)
        {
            _context = context;
        }

        public async Task<Book> CreateBook(Book book)
        {
            await _context.Books.AddAsync(book);
            await _context.SaveChangesAsync();
            return book;
        }

        public async Task Delete(int? bookId)
        {
            var book = await GetById(bookId);
            if (book != null)
            {
                _context.Books.Remove(book);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Book>> GetAll(string? title, int? publisher,
               int? category, string? author, string? language)
        {
            var booksQuery = _context.Books
                .Include(b => b.Author)
                .Include(b => b.Publisher)
                .Include(b => b.Category)
                .AsQueryable();

            if (!string.IsNullOrEmpty(title))
            {
                booksQuery = booksQuery.Where(b =>
                    EF.Functions.Like(
                        EF.Functions.Collate(b.Title, "Latin1_General_CI_AI"),
                        $"%{title}%"));
            }

            if (publisher.HasValue)
            {
                booksQuery = booksQuery.Where(b => b.Publisher.PublisherId == publisher.Value);
            }

            if (category.HasValue)
            {
                booksQuery = booksQuery.Where(b => b.Category.CategoryId == category.Value);
            }

            if (!string.IsNullOrEmpty(author))
            {
                booksQuery = booksQuery.Where(b =>
                    EF.Functions.Like(
                        EF.Functions.Collate(b.Author.AuthorName, "Latin1_General_CI_AI"),
                        $"%{author}%"));
            }

            if (!string.IsNullOrEmpty(language))
            {
                booksQuery = booksQuery.Where(b =>
                    EF.Functions.Like(
                        EF.Functions.Collate(b.Language, "Latin1_General_CI_AI"),
                        $"%{language}%"));
            }

            return await booksQuery.ToListAsync();
        }

        public async Task<List<Book>> GetAllBook()
        {
            return await _context.Books
                .Include(b => b.Author)
                .Include(b => b.Publisher)
                .Include(b => b.Category).ToListAsync();
        }

        public async Task<Book?> GetById(int? id)
        {
            return await _context.Books
                .Include(b => b.Category)
                .Include(b => b.Author)
                .Include(b => b.Publisher)
                .FirstOrDefaultAsync(b => b.BookId == id);
        }

        public async Task UpdateBook(Book book)
        {
            _context.Books.Update(book);
            await _context.SaveChangesAsync();
        }

        // ✅ THÊM MỚI: Method phân trang
        public async Task<(List<Book> books, int totalCount)> GetAllPaginated(
            string? title, int? publisher, int? category, 
            string? author, string? language, int page, int pageSize)
        {
            var booksQuery = _context.Books
                .Include(b => b.Author)
                .Include(b => b.Publisher)
                .Include(b => b.Category)
                .AsQueryable();

            // Áp dụng filters (giống như method GetAll)
            if (!string.IsNullOrEmpty(title))
            {
                booksQuery = booksQuery.Where(b =>
                    EF.Functions.Like(
                        EF.Functions.Collate(b.Title, "Latin1_General_CI_AI"),
                        $"%{title}%"));
            }

            if (publisher.HasValue)
            {
                booksQuery = booksQuery.Where(b => b.Publisher.PublisherId == publisher.Value);
            }

            if (category.HasValue)
            {
                booksQuery = booksQuery.Where(b => b.Category.CategoryId == category.Value);
            }

            if (!string.IsNullOrEmpty(author))
            {
                booksQuery = booksQuery.Where(b =>
                    EF.Functions.Like(
                        EF.Functions.Collate(b.Author.AuthorName, "Latin1_General_CI_AI"),
                        $"%{author}%"));
            }

            if (!string.IsNullOrEmpty(language))
            {
                booksQuery = booksQuery.Where(b =>
                    EF.Functions.Like(
                        EF.Functions.Collate(b.Language, "Latin1_General_CI_AI"),
                        $"%{language}%"));
            }

            // Đếm tổng số records
            int totalCount = await booksQuery.CountAsync();

            // Áp dụng phân trang
            var books = await booksQuery
                .OrderBy(b => b.Title) // Sắp xếp để đảm bảo consistent pagination
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (books, totalCount);
        }

        public async Task Save()
        {
            await _context.SaveChangesAsync();
        }
    }
}
