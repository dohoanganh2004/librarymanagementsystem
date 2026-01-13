namespace ELibrary.WebApp.FileServices
{
    public  static class FileHelper
    {
        public static async Task<string> UploadFileAsync(IFormFile file, string folderPath, string webRootPath)
        {
            try
            {
                // Basic validation
                if (file == null || file.Length == 0)
                {
                    return string.Empty;
                }

                Console.WriteLine($"📤 Uploading: {file.FileName} ({file.Length} bytes)");

                // Simple size check (10MB max for safety)
                if (file.Length > 10 * 1024 * 1024)
                {
                    throw new ArgumentException("File quá lớn (tối đa 10MB)");
                }

                // Simple extension check
                var extension = Path.GetExtension(file.FileName)?.ToLowerInvariant();
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                
                if (string.IsNullOrEmpty(extension) || !allowedExtensions.Contains(extension))
                {
                    throw new ArgumentException("Chỉ chấp nhận file JPG, PNG, GIF");
                }

                // Create directory if not exists
                var uploadPath = Path.Combine(webRootPath, folderPath);
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                // Generate simple filename
                var fileName = $"{DateTime.Now:yyyyMMdd_HHmmss}_{Guid.NewGuid():N}{extension}";
                var fullPath = Path.Combine(uploadPath, fileName);

                Console.WriteLine($"💾 Saving to: {fullPath}");

                // Simple file copy
                using (var fileStream = new FileStream(fullPath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                // Verify file exists
                if (!File.Exists(fullPath))
                {
                    throw new IOException("File không được tạo");
                }

                var relativePath = $"/{folderPath.Replace("\\", "/")}/{fileName}";
                Console.WriteLine($"✅ Upload thành công: {relativePath}");
                
                return relativePath;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Upload lỗi: {ex.Message}");
                throw new Exception($"Không thể upload file: {ex.Message}");
            }
        }

        // Xóa file (nếu cần)
        public static void DeleteFile(string filePath, string webRootPath)
        {
            try
            {
                if (string.IsNullOrEmpty(filePath)) return;

                var fullPath = Path.Combine(webRootPath, filePath.TrimStart('/').Replace("/", "\\"));
                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                    Console.WriteLine($"✅ File deleted: {fullPath}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error deleting file: {ex.Message}");
            }
        }
    }

}
