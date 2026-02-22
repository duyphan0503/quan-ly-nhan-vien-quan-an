using System;
using System.Configuration;
using System.Data.SqlClient;

namespace QuanLyNhanVien.DataAccess
{
    /// <summary>
    /// Factory tạo kết nối trực tiếp với CSDL thiết kế chuẩn Thread-safe.
    /// Ứng dụng Lazy&lt;T&gt; giúp khởi tạo an toàn chuỗi kết nối duy nhất 1 lần (one-time initialization)
    ///
    /// Nâng cấp khả năng cập nhật chuỗi cấu hình tại thời gian thực thông qua Wizard Kết nối trực quan:
    /// khi Wizard lưu trực tiếp thông số vào file App.config rồi khởi động tiến trình RefreshConnectionString(),
    /// Các tác vụ GetConnection() nối tiếp ngay sau đó sẽ tự động sử dụng tham chiếu thiết lập mới.
    /// </summary>
    public static class DatabaseHelper
    {
        private static readonly object _lock = new object();
        private static string _connectionString;
        private static bool _initialized;

        /// <summary>
        /// Biến trỏ thông tin chuỗi kết nối, khởi tạo trễ có sẵn qua tham số được đặt ở App.config.
        /// Được tối ưu an toàn khi chia sẻ trên luồng - Thread-safe.
        /// </summary>
        public static string ConnectionString
        {
            get
            {
                if (!_initialized)
                {
                    lock (_lock)
                    {
                        if (!_initialized)
                        {
                            var cs = ConfigurationManager.ConnectionStrings["QuanLyNhanVien"];
                            if (cs == null)
                                throw new InvalidOperationException(
                                    "Connection string 'QuanLyNhanVien' not found in App.config."
                                );
                            _connectionString = cs.ConnectionString;
                            _initialized = true;
                        }
                    }
                }
                return _connectionString;
            }
        }

        /// <summary>
        /// Khởi tạo phiên giao dịch chuẩn SqlConnection mới. Tiến trình gọi cần tự chịu trách nhiệm Open() và Dispose().
        /// Hàm này không có bất kỳ trạng thái chung - shared mutable state.
        /// </summary>
        public static SqlConnection GetConnection()
        {
            return new SqlConnection(ConnectionString);
        }

        /// <summary>
        /// Ép buộc hệ thống đọc lại chuỗi thông số kết nối từ file App.config (Restarting State).
        /// Hàm này lập tức được thực thi ngay sau khi Connection Wizard lưu sự thay đổi.
        /// </summary>
        public static void RefreshConnectionString()
        {
            lock (_lock)
            {
                _initialized = false;
                _connectionString = null;
                ConfigurationManager.RefreshSection("connectionStrings");
            }
        }

        /// <summary>
        /// Test đường truyền siêu tốc. Hàm phản hồi True nếu có thể cấu trúc liên kết và gọi truy vấn tối giản nhất.
        /// Hàm này được dùng trong lớp cục bộ Program.Main để quyết định xem liệu hệ thống
        /// có nên triệu gọi Wizard sửa lỗi kết nối hay không.
        /// </summary>
        public static bool TestConnection(int timeoutSeconds = 3)
        {
            try
            {
                var builder = new SqlConnectionStringBuilder(ConnectionString);
                builder.ConnectTimeout = timeoutSeconds;

                using (var conn = new SqlConnection(builder.ToString()))
                {
                    conn.Open();
                    using (var cmd = new SqlCommand("SELECT 1", conn))
                    {
                        cmd.ExecuteScalar();
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
