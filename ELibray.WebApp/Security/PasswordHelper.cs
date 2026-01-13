using System.Security.Cryptography;

namespace ELibrary.WebApp.Security
{
    public static class PasswordHelper
    {
        private const int SaltSize = 16;
        private const int HashSize = 32;
        private const int Iterations = 100000;
        private static readonly HashAlgorithmName Algorithm = HashAlgorithmName.SHA256;

        public static string HashPassword(string password)
        {
            // 1. Tạo Salt ngẫu nhiên
            byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);

            // 2. Tính Hash
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, Algorithm))
            {
                byte[] hash = pbkdf2.GetBytes(HashSize);

                // 3. Kết hợp Salt và Hash
                byte[] hashBytes = new byte[SaltSize + HashSize];
                Array.Copy(salt, 0, hashBytes, 0, SaltSize);
                Array.Copy(hash, 0, hashBytes, SaltSize, HashSize);

                // 4. Mã hóa Base64 để lưu vào DB
                return Convert.ToBase64String(hashBytes);
            }
        }

        public static bool VerifyPassword(string password, string storedHash)
        {
            // 1. Giải mã Base64 thành byte array (Salt + Hash)
            byte[] hashBytes;
            try
            {
                hashBytes = Convert.FromBase64String(storedHash);
            }
            catch
            {
                return false; // Trả về false nếu hash lưu trữ không hợp lệ
            }

            // Kiểm tra độ dài: 16 (salt) + 32 (hash) = 48
            if (hashBytes.Length != SaltSize + HashSize)
            {
                return false;
            }

            // 2. Tách Salt
            byte[] salt = new byte[SaltSize];
            Array.Copy(hashBytes, 0, salt, 0, SaltSize);

            // 3. Tách Hash lưu trữ (phần Hash thực tế bắt đầu từ byte thứ 16)
            byte[] storedPasswordHash = new byte[HashSize];
            Array.Copy(hashBytes, SaltSize, storedPasswordHash, 0, HashSize);

            // 4. Tính Hash lại từ mật khẩu người dùng nhập và Salt đã lưu
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, Algorithm))
            {
                byte[] computedHash = pbkdf2.GetBytes(HashSize);

                // 5. SO SÁNH AN TOÀN (FixedTimeEquals)
                // Lỗi của bạn nằm ở đây, cần so sánh computedHash với storedPasswordHash
                // Sử dụng FixedTimeEquals để ngăn chặn timing attacks.
                return CryptographicOperations.FixedTimeEquals(computedHash, storedPasswordHash);
            }
        }
    }
}