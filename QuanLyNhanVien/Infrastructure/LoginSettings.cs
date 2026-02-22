using System;
using System.IO;

namespace QuanLyNhanVien.Infrastructure
{
    public class LoginSettings
    {
        private static readonly string FilePath = "login.cfg";

        public string Username { get; set; }
        public string EncryptedPassword { get; set; }
        public bool RememberMe { get; set; }

        public static void Save(string user, string pass, bool remember)
        {
            try
            {
                // Nếu không ghi nhớ, chúng ta xóa file cũ đi
                if (!remember)
                {
                    if (File.Exists(FilePath))
                        File.Delete(FilePath);
                    return;
                }

                // Nếu ghi nhớ, mã hóa mật khẩu và lưu vào file
                string encryptedPass = SecurityHelper.Encrypt(pass);
                string content = $"{user}|{encryptedPass}|{remember}";
                File.WriteAllText(FilePath, content);
            }
            catch
            { /* Lên log bắt lỗi ở đây nếu cần */
            }
        }

        public static LoginSettings Load()
        {
            try
            {
                if (!File.Exists(FilePath))
                    return null;

                string content = File.ReadAllText(FilePath);
                string[] parts = content.Split('|');

                if (parts.Length == 3)
                {
                    return new LoginSettings
                    {
                        Username = parts[0],
                        EncryptedPassword = parts[1],
                        RememberMe = bool.Parse(parts[2]),
                    };
                }
            }
            catch { }
            return null;
        }
    }
}
