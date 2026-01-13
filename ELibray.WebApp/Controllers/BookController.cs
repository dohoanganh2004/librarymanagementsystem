using E_Library.Models;
using ELibrary.WebApp.DTO.Request;
using ELibrary.WebApp.Excel;
using ELibrary.WebApp.Models;
using ELibrary.WebApp.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Text.Json;

namespace ELibrary.WebApp.Controllers
{
    public class BookController : Controller
    {
        private readonly IAuthorService _authorService;
        private readonly IBookService _bookService;
        private readonly ICategoryService _categoryService;
        private readonly IPublisherService _publisherService;
        private readonly IBookExcelService _bookExcelService;
        private readonly ILogger<BookController> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IAuditLogService _auditLogService;

        public BookController(
            IAuthorService authorService,
            IBookService bookService,
            ICategoryService categoryService,
            IPublisherService publisherService,
            IBookExcelService bookExcelService,
            ILogger<BookController> logger,
            IWebHostEnvironment webHostEnvironment,
            IAuditLogService auditLogService)
        {
            _authorService = authorService;
            _bookService = bookService;
            _categoryService = categoryService;
            _publisherService = publisherService;
            _bookExcelService = bookExcelService;
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
            _auditLogService = auditLogService;
        }

        // Helper method to get current employee ID from session
        private int? GetCurrentEmployeeId()
        {
            return HttpContext.Session.GetInt32("employeeId");
        }

        // Helper method for image validation
        private bool ValidateImageFile(IFormFile imageFile, ModelStateDictionary modelState)
        {
            if (imageFile == null) return true; // No file is OK

            try
            {
                _logger.LogInformation($"Validating image file: {imageFile.FileName} ({imageFile.Length} bytes)");

                // Check file size (50MB max)
                const long maxFileSize = 50 * 1024 * 1024;
                if (imageFile.Length > maxFileSize)
                {
                    modelState.AddModelError("ImageFile", $"File quá lớn. Kích thước tối đa: {maxFileSize / (1024 * 1024)}MB");
                    return false;
                }

                // Check content type
                var allowedTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/gif", "image/bmp", "image/webp" };
                if (string.IsNullOrEmpty(imageFile.ContentType) || !allowedTypes.Contains(imageFile.ContentType.ToLowerInvariant()))
                {
                    modelState.AddModelError("ImageFile", "Chỉ chấp nhận file ảnh (JPG, PNG, GIF, BMP, WEBP)");
                    return false;
                }

                // Check file extension
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };
                var fileExtension = Path.GetExtension(imageFile.FileName)?.ToLowerInvariant();
                if (string.IsNullOrEmpty(fileExtension) || !allowedExtensions.Contains(fileExtension))
                {
                    modelState.AddModelError("ImageFile", "Định dạng file không được hỗ trợ");
                    return false;
                }

                _logger.LogInformation("✅ Image file validation passed");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating image file");
                modelState.AddModelError("ImageFile", "Lỗi khi kiểm tra file ảnh");
                return false;
            }
        }

        // ================== VIEW LIST BOOK (Reader) ==================
        public async Task<IActionResult> All(string? title, int? publisher, int? category, string? author, string? language, int page = 1, int pageSize = 12)
        {
            ViewBag.Categories = await _categoryService.GetAll();
            ViewBag.Publishers = await _publisherService.GetAll();

            
            var searchParams = new BookSearchParameters
            {
                Title = title,
                PublisherId = publisher,
                CategoryId = category,
                Author = author,
                Language = language,
                Page = page,
                PageSize = pageSize
            };

            var paginatedBooks = await _bookService.GetAllPaginated(searchParams);
            
            // Lưu search parameters để giữ lại khi chuyển trang
            ViewBag.CurrentSearch = searchParams;
            
            return View(paginatedBooks);
        }

        // ================== VIEW LIST BOOK (Admin) ==================
        public async Task<IActionResult> List(string? title, int? publisher, int? category, string? author, string? language, int page = 1, int pageSize = 10)
        {
            ViewBag.Categories = await _categoryService.GetAll();
            ViewBag.Publishers = await _publisherService.GetAll();

            
            var searchParams = new BookSearchParameters
            {
                Title = title,
                PublisherId = publisher,
                CategoryId = category,
                Author = author,
                Language = language,
                Page = page,
                PageSize = pageSize
            };

            var paginatedBooks = await _bookService.GetAllPaginated(searchParams);
            
           
            ViewBag.CurrentSearch = searchParams;
            
            return View(paginatedBooks);
        }

        // ================== VIEW DETAIL ==================
        public async Task<IActionResult> Detail(int id)
        {
            var book = await _bookService.GetBook(id);
            return View(book);
        }

        // ================== CREATE BOOK ==================
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadDropdowns();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(BookRequestDTO dto)
        {
            try
            {
                _logger.LogInformation("=== STARTING CREATE BOOK ===");
                _logger.LogInformation($"Title: {dto.Title}");
                _logger.LogInformation($"PublicationYear: {dto.PublicationYear}");
                _logger.LogInformation($"PageCount: {dto.PageCount}");
                _logger.LogInformation($"Quantity: {dto.Quantity}");
                _logger.LogInformation($"AuthorId: {dto.AuthorId}");
                _logger.LogInformation($"CategoryId: {dto.CategoryId}");
                _logger.LogInformation($"PublisherId: {dto.PublisherId}");
                _logger.LogInformation($"ImageFile: {(dto.ImageFile != null ? dto.ImageFile.FileName : "null")}");
                _logger.LogInformation($"Image URL: {dto.Image}");

                // Kiểm tra file upload trước
                if (dto.ImageFile != null)
                {
                    _logger.LogInformation($"File size: {dto.ImageFile.Length} bytes");
                    _logger.LogInformation($"Content type: {dto.ImageFile.ContentType}");
                    
                    // Kiểm tra file type
                    var allowedTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/gif" };
                    if (!allowedTypes.Contains(dto.ImageFile.ContentType.ToLower()))
                    {
                        ModelState.AddModelError("ImageFile", "Chỉ chấp nhận file ảnh (JPG, PNG, GIF)");
                        await LoadDropdowns();
                        return View(dto);
                    }
                    
                    // Kiểm tra kích thước file (max 5MB)
                    if (dto.ImageFile.Length > 5 * 1024 * 1024)
                    {
                        ModelState.AddModelError("ImageFile", "File ảnh không được vượt quá 5MB");
                        await LoadDropdowns();
                        return View(dto);
                    }
                }

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("ModelState is invalid:");
                    foreach (var error in ModelState)
                    {
                        _logger.LogWarning($"Key: {error.Key}, Errors: {string.Join(", ", error.Value.Errors.Select(e => e.ErrorMessage))}");
                    }
                    await LoadDropdowns();
                    return View(dto);
                }

                var result = await _bookService.CreateBook(dto);
                _logger.LogInformation($"Book created successfully with ID: {result.BookId}");

                // Log audit trail
                var employeeId = GetCurrentEmployeeId();
                if (employeeId.HasValue)
                {
                    await _auditLogService.LogActionAsync(
                        employeeId.Value,
                        $"Tạo sách mới: {result.Title}",
                        "Books",
                        result.BookId,
                        null,
                        JsonSerializer.Serialize(new { 
                            BookId = result.BookId,
                            Title = result.Title,
                            Author = result.AuthorName,
                            Category = result.CategoryName,
                            Publisher = result.PublisherName
                        })
                    );
                }
                
                TempData["success"] = "Tạo sách thành công!";
                return RedirectToAction("List");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ CRITICAL ERROR when creating book");
                _logger.LogError($"Exception Type: {ex.GetType().Name}");
                _logger.LogError($"Message: {ex.Message}");
                _logger.LogError($"StackTrace: {ex.StackTrace}");

                if (ex.InnerException != null)
                {
                    _logger.LogError($"Inner Exception: {ex.InnerException.Message}");
                }

                ModelState.AddModelError(string.Empty, $"Có lỗi xảy ra: {ex.Message}");
                await LoadDropdowns();
                return View(dto);
            }
        }


        // ================== UPDATE BOOK ==================
        [HttpGet]
        public async Task<IActionResult> Update(int? id)
        {
            if (id == null) return NotFound();

            var book = await _bookService.GetBook(id);
            if (book == null) return NotFound();

            // Convert BookResponseDTO to BookRequestDTO for editing
            var bookRequest = new BookRequestDTO
            {
                BookId = book.BookId,
                Title = book.Title,
                Description = book.Description,
                CategoryId = book.CategoryId,
                PublisherId = book.PublisherId,
                AuthorId = book.AuthorId,
                Language = book.Language,
                Quantity = book.Quantity ?? 0,
                Image = book.Image,
                PublicationYear = book.PublicationYear ?? DateTime.Now.Year,
                PageCount = book.PageCount ?? 0,
                Status = book.Status
            };

            await LoadDropdowns();
            return View(bookRequest);
        }

        [HttpPost]
        public async Task<IActionResult> Update(BookRequestDTO dto)
        {
            try
            {
                _logger.LogInformation("=== STARTING UPDATE BOOK ===");
                _logger.LogInformation($"BookId: {dto.BookId}");
                _logger.LogInformation($"Title: {dto.Title}");
                _logger.LogInformation($"ImageFile: {(dto.ImageFile != null ? dto.ImageFile.FileName : "null")}");
                _logger.LogInformation($"Image URL: {dto.Image}");

                // Kiểm tra file upload trước
                if (dto.ImageFile != null)
                {
                    _logger.LogInformation($"File size: {dto.ImageFile.Length} bytes");
                    _logger.LogInformation($"Content type: {dto.ImageFile.ContentType}");
                    
                    // Kiểm tra file type
                    var allowedTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/gif" };
                    if (!allowedTypes.Contains(dto.ImageFile.ContentType.ToLower()))
                    {
                        ModelState.AddModelError("ImageFile", "Chỉ chấp nhận file ảnh (JPG, PNG, GIF)");
                        await LoadDropdowns();
                        return View(dto);
                    }
                    
                    // Kiểm tra kích thước file (max 5MB)
                    if (dto.ImageFile.Length > 5 * 1024 * 1024)
                    {
                        ModelState.AddModelError("ImageFile", "File ảnh không được vượt quá 5MB");
                        await LoadDropdowns();
                        return View(dto);
                    }
                }

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("ModelState is invalid:");
                    foreach (var error in ModelState)
                    {
                        _logger.LogWarning($"Key: {error.Key}, Errors: {string.Join(", ", error.Value.Errors.Select(e => e.ErrorMessage))}");
                    }
                    await LoadDropdowns();
                    return View(dto);
                }

                // Get old data before update for logging
                var oldBook = await _bookService.GetBook(dto.BookId);
                
                var result = await _bookService.UpdateBook(dto);
                if (result == null)
                {
                    ModelState.AddModelError("", "Cập nhật thất bại.");
                    await LoadDropdowns();
                    return View(dto);
                }

                _logger.LogInformation($"Book updated successfully with ID: {result.BookId}");

                // Log audit trail
                var employeeId = GetCurrentEmployeeId();
                if (employeeId.HasValue)
                {
                    await _auditLogService.LogActionAsync(
                        employeeId.Value,
                        $"Cập nhật sách: {result.Title}",
                        "Books",
                        result.BookId,
                        JsonSerializer.Serialize(new { 
                            Title = oldBook?.Title,
                            Author = oldBook?.AuthorName,
                            Category = oldBook?.CategoryName,
                            Publisher = oldBook?.PublisherName,
                            Quantity = oldBook?.Quantity
                        }),
                        JsonSerializer.Serialize(new { 
                            Title = result.Title,
                            Author = result.AuthorName,
                            Category = result.CategoryName,
                            Publisher = result.PublisherName,
                            Quantity = result.Quantity
                        })
                    );
                }

                TempData["success"] = "Cập nhật sách thành công!";
                return RedirectToAction("List");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ CRITICAL ERROR when updating book");
                _logger.LogError($"Exception Type: {ex.GetType().Name}");
                _logger.LogError($"Message: {ex.Message}");
                _logger.LogError($"StackTrace: {ex.StackTrace}");

                if (ex.InnerException != null)
                {
                    _logger.LogError($"Inner Exception: {ex.InnerException.Message}");
                }

                ModelState.AddModelError(string.Empty, $"Có lỗi xảy ra: {ex.Message}");
                await LoadDropdowns();
                return View(dto);
            }
        }

        // ================== DELETE BOOK ==================
        public async Task<IActionResult> Delete(int? id)
        {
            try
            {
                if (!id.HasValue)
                {
                    TempData["error"] = "ID sách không hợp lệ";
                    return RedirectToAction("List");
                }

                // Get book data before deletion for logging
                var bookToDelete = await _bookService.GetBook(id.Value);
                if (bookToDelete == null)
                {
                    TempData["error"] = "Không tìm thấy sách để xóa";
                    return RedirectToAction("List");
                }

                // Delete the book
                await _bookService.DeleteBook(id);

                // Log audit trail
                var employeeId = GetCurrentEmployeeId();
                if (employeeId.HasValue)
                {
                    await _auditLogService.LogActionAsync(
                        employeeId.Value,
                        $"Xóa sách: {bookToDelete.Title}",
                        "Books",
                        bookToDelete.BookId,
                        JsonSerializer.Serialize(new { 
                            BookId = bookToDelete.BookId,
                            Title = bookToDelete.Title,
                            Author = bookToDelete.AuthorName,
                            Category = bookToDelete.CategoryName,
                            Publisher = bookToDelete.PublisherName,
                            Quantity = bookToDelete.Quantity,
                            PublicationYear = bookToDelete.PublicationYear
                        }),
                        null // No new data for deletion
                    );
                }

                _logger.LogInformation($"Book deleted successfully: {bookToDelete.Title} (ID: {id})");
                TempData["success"] = $"Đã xóa sách '{bookToDelete.Title}' thành công!";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting book with ID: {id}");
                TempData["error"] = $"Lỗi khi xóa sách: {ex.Message}";
            }

            return RedirectToAction("List");
        }

        // ================== TEST UPLOAD ==================
        [HttpPost]
        public async Task<IActionResult> TestUpload(IFormFile testFile)
        {
            try
            {
                _logger.LogInformation("=== TEST UPLOAD START ===");
                
                if (testFile == null)
                {
                    return Json(new { success = false, message = "No file provided" });
                }

                _logger.LogInformation($"File: {testFile.FileName}");
                _logger.LogInformation($"Size: {testFile.Length} bytes");
                _logger.LogInformation($"Type: {testFile.ContentType}");
                _logger.LogInformation($"WebRootPath: {_webHostEnvironment?.WebRootPath ?? "NULL"}");

                // Simple test - just save to temp
                var tempPath = Path.GetTempFileName();
                using (var stream = new FileStream(tempPath, FileMode.Create))
                {
                    await testFile.CopyToAsync(stream);
                }

                var fileInfo = new FileInfo(tempPath);
                System.IO.File.Delete(tempPath);

                return Json(new { 
                    success = true, 
                    message = "Test upload successful",
                    originalSize = testFile.Length,
                    savedSize = fileInfo.Length
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Test upload failed");
                return Json(new { success = false, message = ex.Message });
            }
        }

        // ================== LOAD DROPDOWNS ==================
        private async Task LoadDropdowns()
        {
            ViewBag.Authors = await _authorService.GetAll();
            ViewBag.Categories = await _categoryService.GetAll();
            ViewBag.Publishers = await _publisherService.GetAll();
        }



        public async Task<IActionResult> ExportExcel()
        {
            try
            {
                _logger.LogInformation("Starting Excel export");
                
                // Check if service is available
                if (_bookExcelService == null)
                {
                    _logger.LogError("BookExcelService is null");
                    TempData["error"] = "Dịch vụ Excel không khả dụng";
                    return RedirectToAction("List");
                }

                var file = await _bookExcelService.ExportBooksToExcelAsync();
                
                if (file == null || file.Length == 0)
                {
                    _logger.LogWarning("Excel export returned empty file");
                    TempData["error"] = "Không thể tạo file Excel";
                    return RedirectToAction("List");
                }
                
                var fileName = $"Books_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                _logger.LogInformation($"Excel export completed: {fileName}, Size: {file.Length} bytes");

                // Log audit trail
                try
                {
                    var employeeId = GetCurrentEmployeeId();
                    if (employeeId.HasValue && _auditLogService != null)
                    {
                        await _auditLogService.LogActionAsync(
                            employeeId.Value,
                            $"Export danh sách sách ra Excel: {fileName}",
                            "Books",
                            null,
                            null,
                            JsonSerializer.Serialize(new { 
                                FileName = fileName,
                                FileSize = file.Length,
                                ExportTime = DateTime.Now
                            })
                        );
                    }
                }
                catch (Exception auditEx)
                {
                    _logger.LogWarning(auditEx, "Failed to log audit trail for Excel export");
                    // Continue with export even if audit logging fails
                }

                return File(
                    file,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    fileName
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Excel export failed: {Message}", ex.Message);
                TempData["error"] = $"Lỗi export Excel: {ex.Message}";
                return RedirectToAction("List");
            }
        }

        // IMPORT - GET (Show form)
        [HttpGet]
        public IActionResult ImportExcel()
        {
            return View();
        }

        // IMPORT - POST (Process file)
        [HttpPost]
        public async Task<IActionResult> ImportExcel(IFormFile excelFile)
        {
            try
            {
                // Validate input
                if (excelFile == null || excelFile.Length == 0)
                {
                    TempData["error"] = "Vui lòng chọn file Excel để import";
                    return View();
                }

                // Validate file extension
                var extension = Path.GetExtension(excelFile.FileName)?.ToLowerInvariant();
                if (extension != ".xlsx" && extension != ".xls")
                {
                    TempData["error"] = "Chỉ chấp nhận file Excel (.xlsx, .xls)";
                    return View();
                }

                // Validate file size (max 10MB)
                if (excelFile.Length > 10 * 1024 * 1024)
                {
                    TempData["error"] = "File Excel không được vượt quá 10MB";
                    return View();
                }

                // Check if service is available
                if (_bookExcelService == null)
                {
                    _logger.LogError("BookExcelService is null");
                    TempData["error"] = "Dịch vụ Excel không khả dụng";
                    return View();
                }

                _logger.LogInformation($"Starting Excel import: {excelFile.FileName}, Size: {excelFile.Length} bytes");
                
                await _bookExcelService.ImportBooksFromExcelAsync(excelFile);

                // Log audit trail
                try
                {
                    var employeeId = GetCurrentEmployeeId();
                    if (employeeId.HasValue && _auditLogService != null)
                    {
                        await _auditLogService.LogActionAsync(
                            employeeId.Value,
                            $"Import sách từ Excel: {excelFile.FileName}",
                            "Books",
                            null,
                            null,
                            JsonSerializer.Serialize(new { 
                                FileName = excelFile.FileName,
                                FileSize = excelFile.Length,
                                ImportTime = DateTime.Now
                            })
                        );
                    }
                }
                catch (Exception auditEx)
                {
                    _logger.LogWarning(auditEx, "Failed to log audit trail for Excel import");
                    // Continue even if audit logging fails
                }
                
                TempData["success"] = "Import Excel thành công!";
                _logger.LogInformation($"Excel import completed successfully: {excelFile.FileName}");
                return RedirectToAction("List");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Excel import failed: {Message}", ex.Message);
                TempData["error"] = $"Lỗi import Excel: {ex.Message}";
                return View();
            }
        }
    }
}
