using AutoMapper;
using E_Library.Models;
using E_Library.Repository;
using ELibrary.WebApp.DTO.Request;
using ELibrary.WebApp.DTO.Response;
using ELibrary.WebApp.FileServices;
using ELibrary.WebApp.Hubs;
using ELibrary.WebApp.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.SignalR;
using OfficeOpenXml;

namespace ELibrary.WebApp.Service
{
    public class BookService : IBookService
    {   
        private readonly IBookRepository bookRepository;
        private readonly IMapper mapper;
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly ILogger<BookService> logger;
       

        public BookService(IBookRepository bookRepository, IMapper mapper, IWebHostEnvironment webHostEnvironment , ILogger<BookService> logger
            )
        {
            this.bookRepository = bookRepository;
            this.mapper = mapper;
            this.webHostEnvironment = webHostEnvironment;
            this.logger = logger;
           
        }

        // create new book (async)
        public async Task<BookResponseDTO> CreateBook(BookRequestDTO bookRequestDTO)
        {
            try
            {
               
                // Validate publication year
                int currentYear = DateTime.Now.Year;
                if (bookRequestDTO.PublicationYear > currentYear)
                {
                    throw new ArgumentException($"Năm xuất bản không thể lớn hơn {currentYear}");
                }

                // Handle image upload with enhanced error handling
                string imageUrl = bookRequestDTO.Image; 
                
                if (bookRequestDTO.ImageFile != null && bookRequestDTO.ImageFile.Length > 0)
                {
                    try
                    {
                        logger.LogInformation($"📸 Processing image file: {bookRequestDTO.ImageFile.FileName}");
                        logger.LogInformation($"📏 File size: {bookRequestDTO.ImageFile.Length} bytes");
                        logger.LogInformation($"🌐 WebRootPath: {webHostEnvironment.WebRootPath}");

                        // Validate WebRootPath
                        if (string.IsNullOrEmpty(webHostEnvironment.WebRootPath))
                        {
                            throw new InvalidOperationException("WebRootPath is not configured");
                        }

                        imageUrl = await FileHelper.UploadFileAsync(
                            bookRequestDTO.ImageFile,
                            "img/books",
                            webHostEnvironment.WebRootPath
                        );

                        logger.LogInformation($"✅ Image uploaded successfully: {imageUrl}");
                    }
                    catch (Exception uploadEx)
                    {
                        logger.LogError(uploadEx, "❌ Image upload failed");
                        throw new Exception($"Không thể upload ảnh: {uploadEx.Message}", uploadEx);
                    }
                }
                else
                {
                    logger.LogInformation("ℹ️ No image file provided, using URL or keeping empty");
                }

                // Create book entity
                Book newBook = new Book
                {
                    Title = bookRequestDTO.Title,
                    Description = bookRequestDTO.Description,
                    CategoryId = bookRequestDTO.CategoryId,
                    PublisherId = bookRequestDTO.PublisherId,
                    AuthorId = bookRequestDTO.AuthorId,
                    Language = bookRequestDTO.Language,
                    Quantity = bookRequestDTO.Quantity,
                    Image = imageUrl,
                    PublicationYear = bookRequestDTO.PublicationYear,
                    PageCount = bookRequestDTO.PageCount,
                    Status = bookRequestDTO.Status ?? true 
                };

                Console.WriteLine(" Saving book to database...");
                await bookRepository.CreateBook(newBook);
                Console.WriteLine($" Book saved successfully with ID: {newBook.BookId}");

                return mapper.Map<BookResponseDTO>(newBook);
            }
            catch (Exception ex)
            {
                logger.LogWarning(" ERROR in CreateBook:");
                logger.LogWarning($"Exception Type: {ex.GetType().Name}");
                logger.LogWarning($"Message: {ex.Message}");
                logger.LogWarning($"Stack Trace: {ex.StackTrace}");

                if (ex.InnerException != null)
                {
                    logger.LogWarning($"Inner Exception: {ex.InnerException.Message}");
                }

                throw; 
            }
        }


        // delete book (async)
        public async Task DeleteBook(int? id)
        {
            await bookRepository.Delete(id);
        }

       
        // get all book (async)
        public async Task<List<BookResponseDTO>> GetAll(
                string? title, int? publisher, int? category,
                string? author, string? language)
        {
            var books = await bookRepository.GetAll(title, publisher, category, author, language);
            return mapper.Map<List<BookResponseDTO>>(books);
        }

        
        public async Task<PaginationModel<BookResponseDTO>> GetAllPaginated(BookSearchParameters parameters)
        {
            try
            {
               
                parameters.Validate();

               
                var (books, totalCount) = await bookRepository.GetAllPaginated(
                    parameters.Title, 
                    parameters.PublisherId, 
                    parameters.CategoryId,
                    parameters.Author, 
                    parameters.Language, 
                    parameters.Page, 
                    parameters.PageSize);

               
                var bookDTOs = mapper.Map<List<BookResponseDTO>>(books);

               
                int totalPages = (int)Math.Ceiling((double)totalCount / parameters.PageSize);

                return new PaginationModel<BookResponseDTO>
                {
                    Items = bookDTOs,
                    CurrentPage = parameters.Page,
                    TotalPages = totalPages,
                    TotalItems = totalCount,
                    PageSize = parameters.PageSize
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($" ERROR in GetAllPaginated: {ex.Message}");
                
                // Trả về kết quả rỗng nếu có lỗi
                return new PaginationModel<BookResponseDTO>
                {
                    Items = new List<BookResponseDTO>(),
                    CurrentPage = 1,
                    TotalPages = 0,
                    TotalItems = 0,
                    PageSize = parameters.PageSize
                };
            }
        }

        // get book by id (async)
        public async Task<BookResponseDTO> GetBook(int? id)
        {
            var book = await bookRepository.GetById(id);
            return mapper.Map<BookResponseDTO>(book);
        }

        // get Book Entity by ID (async)
        public async Task<Book> GetBookByID(int? id)
        {
            return await bookRepository.GetById(id);
        }

     
        // update book (async)
        public async Task<BookResponseDTO> UpdateBook(BookRequestDTO bookRequestDTO)
        {
            try
            {
                Console.WriteLine("🔄 Starting UpdateBook in BookService");
                Console.WriteLine($"📝 BookId: {bookRequestDTO.BookId}");
                Console.WriteLine($"📝 Title: {bookRequestDTO.Title}");

                // Lấy book hiện tại từ database
                var existingBook = await bookRepository.GetById(bookRequestDTO.BookId);
                if (existingBook == null)
                {
                    throw new ArgumentException("Không tìm thấy sách để cập nhật");
                }

                Console.WriteLine($"✅ Found existing book: {existingBook.Title}");

                // Xử lý upload file nếu có
                string imageUrl = bookRequestDTO.Image; // Keep current URL if provided
                
                if (bookRequestDTO.ImageFile != null && bookRequestDTO.ImageFile.Length > 0)
                {
                    Console.WriteLine($"📸 Processing new image file: {bookRequestDTO.ImageFile.FileName}");
                    Console.WriteLine($"📏 File size: {bookRequestDTO.ImageFile.Length} bytes");

                    imageUrl = await FileHelper.UploadFileAsync(
                        bookRequestDTO.ImageFile,
                        "img/books",
                        webHostEnvironment.WebRootPath
                    );
                    
                    Console.WriteLine($"✅ New image uploaded successfully: {imageUrl}");
                }
                else if (string.IsNullOrEmpty(bookRequestDTO.Image))
                {
                    // Keep existing image if no new image provided
                    imageUrl = existingBook.Image;
                    Console.WriteLine($"ℹ️ Keeping existing image: {imageUrl}");
                }

                // Cập nhật các thuộc tính
                existingBook.Title = bookRequestDTO.Title;
                existingBook.Description = bookRequestDTO.Description;
                existingBook.CategoryId = bookRequestDTO.CategoryId;
                existingBook.PublisherId = bookRequestDTO.PublisherId;
                existingBook.AuthorId = bookRequestDTO.AuthorId;
                existingBook.Language = bookRequestDTO.Language;
                existingBook.Quantity = bookRequestDTO.Quantity;
                existingBook.PageCount = bookRequestDTO.PageCount;
                existingBook.PublicationYear = bookRequestDTO.PublicationYear;
                existingBook.Status = bookRequestDTO.Status ?? existingBook.Status;
                existingBook.Image = imageUrl;

                Console.WriteLine("💾 Updating book in database...");
                await bookRepository.UpdateBook(existingBook);
                Console.WriteLine($"✅ Book updated successfully with ID: {existingBook.BookId}");

                return mapper.Map<BookResponseDTO>(existingBook);
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ ERROR in UpdateBook:");
                Console.WriteLine($"Exception Type: {ex.GetType().Name}");
                Console.WriteLine($"Message: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");

                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }

                throw;
            }
        }
        
        

    }
}
