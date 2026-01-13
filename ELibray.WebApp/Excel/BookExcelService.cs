using E_Library.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace ELibrary.WebApp.Excel
{
    public class BookExcelService : IBookExcelService
    {
        private readonly ElibraryContext _context;

        public BookExcelService(ElibraryContext context)
        {
            _context = context;
        }

        // ================= EXPORT ====================
        public async Task<byte[]> ExportBooksToExcelAsync()
        {
            try
            {
                // Set EPPlus license context
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                var books = await _context.Books
                    .Include(b => b.Author)
                    .Include(b => b.Category)
                    .Include(b => b.Publisher)
                    .Where(b => b.Status == true) // Only export active books
                    .OrderBy(b => b.BookId)
                    .ToListAsync();

                if (!books.Any())
                {
                    throw new Exception("Không có sách nào để export");
                }

                using var package = new ExcelPackage();
                var ws = package.Workbook.Worksheets.Add("Danh Sách Sách");

                // Header with styling
                ws.Cells["A1"].Value = "Mã Sách";
                ws.Cells["B1"].Value = "Tên Sách";
                ws.Cells["C1"].Value = "Tác Giả";
                ws.Cells["D1"].Value = "Thể Loại";
                ws.Cells["E1"].Value = "Nhà Xuất Bản";
                ws.Cells["F1"].Value = "Năm Xuất Bản";
                ws.Cells["G1"].Value = "Số Lượng";
                ws.Cells["H1"].Value = "Mô Tả";
                
                // Style header
                using (var range = ws.Cells["A1:H1"])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(79, 129, 189));
                    range.Style.Font.Color.SetColor(System.Drawing.Color.White);
                    range.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                }

                // Data rows
                int row = 2;
                foreach (var book in books)
                {
                    ws.Cells[row, 1].Value = book.BookId;
                    ws.Cells[row, 2].Value = book.Title ?? "";
                    ws.Cells[row, 3].Value = book.Author?.AuthorName ?? "";
                    ws.Cells[row, 4].Value = book.Category?.CategoryName ?? "";
                    ws.Cells[row, 5].Value = book.Publisher?.PublisherName ?? "";
                    ws.Cells[row, 6].Value = book.PublicationYear ?? 0;
                    ws.Cells[row, 7].Value = book.Quantity ?? 0;
                    ws.Cells[row, 8].Value = book.Description ?? "";
                    
                    // Add borders to data rows
                    using (var range = ws.Cells[row, 1, row, 8])
                    {
                        range.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    }
                    
                    row++;
                }

                // Auto-fit columns
                ws.Cells.AutoFitColumns();
                
                // Set minimum column widths
                for (int col = 1; col <= 8; col++)
                {
                    if (ws.Column(col).Width < 10)
                        ws.Column(col).Width = 10;
                }

                var result = package.GetAsByteArray();
                
                if (result == null || result.Length == 0)
                {
                    throw new Exception("Không thể tạo file Excel");
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi export Excel: {ex.Message}", ex);
            }
        }

        // ================= IMPORT ====================
        public async Task ImportBooksFromExcelAsync(IFormFile excelFile)
        {
            try
            {
                // Set EPPlus license context
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                if (excelFile == null || excelFile.Length == 0)
                {
                    throw new Exception("File Excel không hợp lệ");
                }

                using var stream = new MemoryStream();
                await excelFile.CopyToAsync(stream);
                stream.Position = 0; // Reset stream position

                using var package = new ExcelPackage(stream);
                
                if (package.Workbook.Worksheets.Count == 0)
                {
                    throw new Exception("File Excel không có worksheet nào");
                }

                var ws = package.Workbook.Worksheets[0];
                
                if (ws.Dimension == null)
                {
                    throw new Exception("Worksheet trống hoặc không có dữ liệu");
                }

                int rowCount = ws.Dimension.Rows;
                if (rowCount < 2)
                {
                    throw new Exception("File Excel phải có ít nhất 2 dòng (header + data)");
                }

                // Validate header format (optional but recommended)
                var expectedHeaders = new[] { "Tên Sách", "Tác Giả", "Thể Loại", "Nhà Xuất Bản", "Năm Xuất Bản", "Số Lượng" };
                // You can add header validation here if needed

                // Collect all unique names first to minimize database calls
                var authorNames = new HashSet<string>();
                var categoryNames = new HashSet<string>();
                var publisherNames = new HashSet<string>();

                // First pass: collect all names
                for (int row = 2; row <= rowCount; row++)
                {
                    try
                    {
                        string title = ws.Cells[row, 2].Text?.Trim();
                        if (string.IsNullOrEmpty(title)) continue; // Skip rows without title

                        string authorName = ws.Cells[row, 3].Text?.Trim();
                        string categoryName = ws.Cells[row, 4].Text?.Trim();
                        string publisherName = ws.Cells[row, 5].Text?.Trim();

                        if (!string.IsNullOrEmpty(authorName)) authorNames.Add(authorName);
                        if (!string.IsNullOrEmpty(categoryName)) categoryNames.Add(categoryName);
                        if (!string.IsNullOrEmpty(publisherName)) publisherNames.Add(publisherName);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"Lỗi đọc dữ liệu tại dòng {row}: {ex.Message}");
                    }
                }

                // Get existing entities
                var existingAuthors = await _context.Authors
                    .Where(a => authorNames.Contains(a.AuthorName))
                    .ToDictionaryAsync(a => a.AuthorName, a => a);

                var existingCategories = await _context.Categories
                    .Where(c => categoryNames.Contains(c.CategoryName))
                    .ToDictionaryAsync(c => c.CategoryName, c => c);

                var existingPublishers = await _context.Publishers
                    .Where(p => publisherNames.Contains(p.PublisherName))
                    .ToDictionaryAsync(p => p.PublisherName, p => p);

                // Create missing entities
                var newAuthors = new List<Author>();
                var newCategories = new List<Category>();
                var newPublishers = new List<Publisher>();

                foreach (var name in authorNames.Where(n => !existingAuthors.ContainsKey(n)))
                {
                    var author = new Author { AuthorName = name };
                    newAuthors.Add(author);
                    existingAuthors[name] = author;
                }

                foreach (var name in categoryNames.Where(n => !existingCategories.ContainsKey(n)))
                {
                    var category = new Category { CategoryName = name };
                    newCategories.Add(category);
                    existingCategories[name] = category;
                }

                foreach (var name in publisherNames.Where(n => !existingPublishers.ContainsKey(n)))
                {
                    var publisher = new Publisher { PublisherName = name };
                    newPublishers.Add(publisher);
                    existingPublishers[name] = publisher;
                }

                // Add new entities to context
                if (newAuthors.Any()) _context.Authors.AddRange(newAuthors);
                if (newCategories.Any()) _context.Categories.AddRange(newCategories);
                if (newPublishers.Any()) _context.Publishers.AddRange(newPublishers);

                await _context.SaveChangesAsync(); // Save to get IDs

                // Second pass: create books
                var booksToAdd = new List<Book>();
                var existingTitles = await _context.Books
                    .Select(b => b.Title.ToLower())
                    .ToHashSetAsync();

                int successCount = 0;
                int skipCount = 0;

                for (int row = 2; row <= rowCount; row++)
                {
                    try
                    {
                        string title = ws.Cells[row, 2].Text?.Trim();
                        if (string.IsNullOrEmpty(title))
                        {
                            skipCount++;
                            continue; // Skip empty titles
                        }

                        // Check for duplicate
                        if (existingTitles.Contains(title.ToLower()))
                        {
                            skipCount++;
                            continue;
                        }

                        string authorName = ws.Cells[row, 3].Text?.Trim();
                        string categoryName = ws.Cells[row, 4].Text?.Trim();
                        string publisherName = ws.Cells[row, 5].Text?.Trim();

                        int.TryParse(ws.Cells[row, 6].Text, out int year);
                        int.TryParse(ws.Cells[row, 7].Text, out int quantity);

                        var book = new Book
                        {
                            Title = title,
                            AuthorId = !string.IsNullOrEmpty(authorName) && existingAuthors.ContainsKey(authorName) 
                                ? existingAuthors[authorName].AuthorId : (int?)null,
                            CategoryId = !string.IsNullOrEmpty(categoryName) && existingCategories.ContainsKey(categoryName) 
                                ? existingCategories[categoryName].CategoryId : (int?)null,
                            PublisherId = !string.IsNullOrEmpty(publisherName) && existingPublishers.ContainsKey(publisherName) 
                                ? existingPublishers[publisherName].PublisherId : (int?)null,
                            PublicationYear = year > 1800 && year <= DateTime.Now.Year + 1 ? year : null,
                            Quantity = quantity >= 0 ? quantity : 0,
                            Status = true,
                            Description = ws.Cells[row, 8].Text?.Trim() // Add description if available
                        };

                        booksToAdd.Add(book);
                        existingTitles.Add(title.ToLower()); // Prevent duplicates within the same import
                        successCount++;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"Lỗi xử lý dữ liệu tại dòng {row}: {ex.Message}");
                    }
                }

                if (booksToAdd.Any())
                {
                    _context.Books.AddRange(booksToAdd);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    throw new Exception($"Không có sách nào được import. Đã bỏ qua {skipCount} dòng (trùng lặp hoặc thiếu dữ liệu)");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi import Excel: {ex.Message}", ex);
            }
        }
    }
}
