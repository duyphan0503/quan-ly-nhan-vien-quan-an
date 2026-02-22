using System;
using System.Security.Cryptography;
using System.Text;

namespace QuanLyNhanVien.Infrastructure
{
    public static class SecurityHelper
    {
        private static readonly byte[] Entropy = Encoding.UTF8.GetBytes(
            "QuanLyNhaHang_Secret_Salt"
        );

        // ================================================================
        // HASH MẬT KHẨU (SHA-256) — dùng khi lưu/so sánh mật khẩu trong DB
        // ================================================================

        /// <summary>
        /// Hash mật khẩu bằng SHA-256. Kết quả là chuỗi hex 64 ký tự (viết thường).
        /// Đây là hàm một chiều — không thể giải mã ngược lại.
        /// Ví dụ: "admin123" → "240be518fabd2724ddb6f04eeb1da5967448d7e831c08c8fa822809f74c720a9"
        /// </summary>
        public static string HashPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
                return null;

            using (var sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));

                // Chuyển byte[] → chuỗi hex (viết thường)
                var sb = new StringBuilder(64);
                foreach (byte b in bytes)
                    sb.Append(b.ToString("x2"));

                return sb.ToString();
            }
        }

        // ================================================================
        // MÃ HÓA DPAPI — dùng cho chức năng "Ghi nhớ đăng nhập" (login.cfg)
        // ================================================================

        public static string Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
                return null;
            try
            {
                byte[] data = Encoding.UTF8.GetBytes(plainText);
                byte[] encryptedData = ProtectedData.Protect(
                    data,
                    Entropy,
                    DataProtectionScope.CurrentUser
                );
                return Convert.ToBase64String(encryptedData);
            }
            catch
            {
                return null;
            }
        }

        public static string Decrypt(string encryptedText)
        {
            if (string.IsNullOrEmpty(encryptedText))
                return null;
            try
            {
                byte[] data = Convert.FromBase64String(encryptedText);
                byte[] decryptedData = ProtectedData.Unprotect(
                    data,
                    Entropy,
                    DataProtectionScope.CurrentUser
                );
                return Encoding.UTF8.GetString(decryptedData);
            }
            catch
            {
                return null;
            }
        }
    }
}
