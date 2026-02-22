using System;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;
using System.Text;

namespace QuanLyNhanVien.Infrastructure
{
    /// <summary>
    /// Mức độ nghiêm trọng của log.
    /// </summary>
    public enum LogLevel
    {
        Info,
        Warning,
        Error,
        Critical,
    }

    /// <summary>
    /// Bộ máy ghi nhật ký (logger) tập trung của toàn ứng dụng.
    /// Kiến trúc xuất kép (Dual-output): ghi cả vào tệp log vật lý Cục bộ VÀ lưu vào bảng ErrorLog trên CSDL.
    ///
    /// Thiết kế:
    /// - Quá trình ghi tệp (File logging) LUÔN hoạt động (kể cả khi CSDL bị sập mạng — đây là phương án dự phòng an toàn)
    /// - Quá trình ghi CSDL hoạt động theo kiểu "cố gắng hết sức" (best-effort) (những lỗi nảy sinh sẽ bị tóm gọn và ghi đè sang tệp cục bộ)
    /// - Thread-safe: xử lý Lock luồng đa nhiệm an toàn để khóa ghi tệp
    /// - Tự động quay vòng (Auto-rotates) tệp log qua từng ngày (một file một ngày để chống tắc nghẽn)
    /// - Tự động bắt lại các thông tin: Tên máy, phần bản ứng dụng, người dùng hiện tại
    /// </summary>
    public static class AppLogger
    {
        // ── Cấu Hình ──
        private static readonly object _fileLock = new object();
        private static string _logDirectory;
        private static string _currentUser;
        private static readonly string _appVersion;
        private static readonly string _machineName;

        static AppLogger()
        {
            _appVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            _machineName = Environment.MachineName;
            _currentUser = null;

            // Thư mục mặc định: [exe_path]/Logs/
            string exeDir = AppDomain.CurrentDomain.BaseDirectory;
            _logDirectory = Path.Combine(exeDir, "Logs");
        }

        /// <summary>
        /// Gán định danh tên người dùng đã vượt qua rào bảo mật vào trong cấu trúc nội dung bản ghi log.
        /// Hãy gọi tới tham số này ngay sau khi có thông báo "Đăng nhập xuất sắc".
        /// </summary>
        public static void SetCurrentUser(string username)
        {
            _currentUser = username;
        }

        /// <summary>
        /// Đè phương thức tùy chỉnh thư mục (dành cho lấy địa chỉ từ một cài đặt cấu hình file nào đó).
        /// </summary>
        public static void SetLogDirectory(string path)
        {
            _logDirectory = path;
        }

        // ── API công khai ──

        public static void Info(string source, string message)
        {
            Log(LogLevel.Info, source, message, null);
        }

        public static void Warning(string source, string message)
        {
            Log(LogLevel.Warning, source, message, null);
        }

        public static void Error(string source, string message, Exception ex = null)
        {
            Log(LogLevel.Error, source, message, ex);
        }

        public static void Critical(string source, string message, Exception ex = null)
        {
            Log(LogLevel.Critical, source, message, ex);
        }

        /// <summary>
        /// Hàm gốc quản lý việc Ghi Nhật Ký. Quá trình tệp cục bộ diễn ra đầu tiên (luôn luôn như vậy), rồi mới nối qua CSDL (best-effort).
        /// </summary>
        public static void Log(LogLevel level, string source, string message, Exception ex)
        {
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            string stackTrace = ex != null ? FlattenException(ex) : null;

            // ── Bước 1: Luôn ghi vào tệp vật lý (Không bao giờ được bỏ qua) ──
            WriteToFile(timestamp, level, source, message, stackTrace);

            // ── Bước 2: Nỗ lực hết mức đổ dữ liệu vào CSDL ──
            try
            {
                WriteToDatabase(level, source, message, stackTrace);
            }
            catch
            {
                // Việc ghi CSDL thất bại — Hoàn toàn bình thường, chúng ta đã có log vật lý phòng hờ.
                // Không throw lỗi Exception ra: Hệ thống sẽ tự sập nếu để điều đó xảy ra.
            }
        }

        // ── Tệp Cục Bộ (Tuyến Xả) ──

        private static void WriteToFile(
            string timestamp,
            LogLevel level,
            string source,
            string message,
            string detail
        )
        {
            try
            {
                if (!Directory.Exists(_logDirectory))
                    Directory.CreateDirectory(_logDirectory);

                string fileName = DateTime.Now.ToString("yyyy-MM-dd") + ".log";
                string filePath = Path.Combine(_logDirectory, fileName);

                var sb = new StringBuilder();
                sb.AppendFormat("[{0}] [{1}] [{2}]", timestamp, level.ToString().ToUpper(), source);
                if (_currentUser != null)
                    sb.AppendFormat(" [User:{0}]", _currentUser);
                sb.AppendLine();
                sb.AppendFormat("  Message: {0}", message);
                sb.AppendLine();

                if (!string.IsNullOrEmpty(detail))
                {
                    sb.AppendFormat("  Detail:  {0}", detail);
                    sb.AppendLine();
                }
                sb.AppendLine("  ---");

                lock (_fileLock)
                {
                    File.AppendAllText(filePath, sb.ToString(), Encoding.UTF8);
                }
            }
            catch
            {
                // Phương án cuối cùng là không thể cứu với ngoại lệ hệ thống tệp — nếu cái này cũng gãy thì
                // thôi mặc kệ. Đừng báo lỗi (Throw) từ Logger.
            }
        }

        // ── Cơ Sở Dữ Liệu ──

        private static void WriteToDatabase(
            LogLevel level,
            string source,
            string message,
            string detail
        )
        {
            // Phụ thuộc vào con đường DatabaseHelper thiết kế. Nếu mất nối (tức là chưa có cấu hình DB nào cả),
            // phương pháp này sẽ gói bên ngoài khối Lệnh Try-Catch tại vị trí gọi tới hàm.
            string connStr = DataAccess.DatabaseHelper.ConnectionString;

            using (var conn = new SqlConnection(connStr))
            {
                conn.Open();
                using (
                    var cmd = new SqlCommand(
                        @"
                    INSERT INTO ErrorLog (MucDo, NguonLoi, ThongBao, ChiTiet, NguoiDung, TenMay, PhienBan)
                    VALUES (@MucDo, @NguonLoi, @ThongBao, @ChiTiet, @NguoiDung, @TenMay, @PhienBan)",
                        conn
                    )
                )
                {
                    cmd.Parameters.AddWithValue("@MucDo", level.ToString());
                    cmd.Parameters.AddWithValue("@NguonLoi", Truncate(source, 200));
                    cmd.Parameters.AddWithValue("@ThongBao", message ?? "");
                    cmd.Parameters.AddWithValue("@ChiTiet", (object)detail ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@NguoiDung", (object)_currentUser ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@TenMay", Truncate(_machineName, 100));
                    cmd.Parameters.AddWithValue("@PhienBan", _appVersion);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // ── Cấu Trúc Hỗ Trợ (Helpers) ──

        /// <summary>
        /// Cấu trúc giải nén mảng chuỗi phân cấp (bao gồm cả lớp bảo vệ InnerException) để đổ ra mảng chuỗi ký tự String thuần.
        /// </summary>
        private static string FlattenException(Exception ex)
        {
            var sb = new StringBuilder();
            int depth = 0;
            Exception current = ex;
            while (current != null && depth < 5)
            {
                if (depth > 0)
                    sb.AppendLine("--- Inner Exception ---");
                sb.AppendFormat("[{0}] {1}", current.GetType().FullName, current.Message);
                sb.AppendLine();
                if (current.StackTrace != null)
                    sb.AppendLine(current.StackTrace);
                current = current.InnerException;
                depth++;
            }
            return sb.ToString();
        }

        private static string Truncate(string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value))
                return value;
            return value.Length <= maxLength ? value : value.Substring(0, maxLength);
        }
    }
}
