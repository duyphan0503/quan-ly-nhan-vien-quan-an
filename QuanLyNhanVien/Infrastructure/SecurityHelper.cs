using System;
using System.Security.Cryptography;
using System.Text;

namespace QuanLyNhanVien.Infrastructure
{
    public static class SecurityHelper
    {
        private static readonly byte[] Entropy = Encoding.UTF8.GetBytes("QuanLyNhaHang_Secret_Salt");
        public static string Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText)) return null;
            try {
                byte[] data = Encoding.UTF8.GetBytes(plainText);
                byte[] encryptedData = ProtectedData.Protect(data, Entropy, DataProtectionScope.CurrentUser);
                return Convert.ToBase64String(encryptedData);
            }
            catch { return null;}
        }

        public static string Decrypt(string encryptedText)
        {
            if (string.IsNullOrEmpty(encryptedText)) return null;
            try 
            {
                byte[] data = Convert.FromBase64String(encryptedText);
                byte[] decryptedData = ProtectedData.Unprotect(data, Entropy, DataProtectionScope.CurrentUser);
                return Encoding.UTF8.GetString(decryptedData);
            }
            catch { return null; }
        }
    }
}