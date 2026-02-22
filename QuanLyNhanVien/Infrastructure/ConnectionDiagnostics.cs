using System;
using System.Data.SqlClient;
using System.Net.Sockets;

namespace QuanLyNhanVien.Infrastructure
{
    /// <summary>
    /// Đối tượng chứa kết quả chuẩn đoán cho từng bước kiểm tra trong wizard cài đặt kết nối.
    /// </summary>
    public class DiagnosticResult
    {
        public bool Success { get; private set; }
        public string StepName { get; private set; }
        public string Message { get; private set; }
        public string Suggestion { get; private set; }
        public int DurationMs { get; private set; }

        private DiagnosticResult() { }

        public static DiagnosticResult Pass(string step, string message, int durationMs = 0)
        {
            return new DiagnosticResult
            {
                Success = true,
                StepName = step,
                Message = message,
                Suggestion = null,
                DurationMs = durationMs,
            };
        }

        public static DiagnosticResult Fail(
            string step,
            string message,
            string suggestion,
            int durationMs = 0
        )
        {
            return new DiagnosticResult
            {
                Success = false,
                StepName = step,
                Message = message,
                Suggestion = suggestion,
                DurationMs = durationMs,
            };
        }
    }

    /// <summary>
    /// File chẩn đoán cấu trúc kết nối với hệ quản trị CSDL theo quy trình máy trạng thái.
    /// Tuân thủ quy trình kiểm tra bảo mật gồm 4 buớc (4-step diagnostic flow):
    ///   1. TCP Connectivity — Có thể kết nối đường truyền mạng vật lý tới CSDL không?
    ///   2. SQL Authentication — Thông tin tài khoản có hợp lệ và được cấp quyền chưa?
    ///   3. Database Existence — Bảng dữ liệu có tồn tại cấu trúc thực sự trên vùng nhớ máy chủ?
    ///   4. Table Verification — Các bảng (Tables) bắt buộc đã khởi tạo (schema initialized) thành công?
    ///
    /// Từng bước sẽ trả về mẫu `DiagnosticResult` tương ứng với mỗi pass/fail
    /// kèm các thông điệp ghi chú chuyên biệt hoặc chỉ thị gỡ rối cho người vận hành trực tuyến.
    /// </summary>
    public static class ConnectionDiagnostics
    {
        /// <summary>
        /// Bước 1: Test raw TCP kết nối trực tiếp đến điểm host:port của SQL Server.
        /// Chặn các nguy cơ tiềm ẩn: Sai địa chỉ IP mạng, CSDL ngưng chạy do lỗi Firewall chặn kết nối cổng mạng hay lỗi kỹ thuật DNS.
        /// </summary>
        public static DiagnosticResult TestTcpConnectivity(
            string server,
            int port,
            int timeoutMs = 3000
        )
        {
            string step = "Kết nối mạng (TCP)";
            var sw = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                using (var client = new TcpClient())
                {
                    var connectTask = client.BeginConnect(server, port, null, null);
                    bool connected = connectTask.AsyncWaitHandle.WaitOne(timeoutMs);
                    sw.Stop();

                    if (!connected)
                    {
                        return DiagnosticResult.Fail(
                            step,
                            string.Format(
                                "Timeout kết nối đến {0}:{1} sau {2}ms.",
                                server,
                                port,
                                timeoutMs
                            ),
                            "Kiểm tra:\n"
                                + "  • Tên máy chủ / IP address có đúng không?\n"
                                + "  • SQL Server đã bật chưa? (Windows Services → SQL Server)\n"
                                + "  • Tường lửa Windows có chặn cổng "
                                + port
                                + " không?\n"
                                + "  • Nếu dùng tên máy, thử dùng IP address thay thế.",
                            (int)sw.ElapsedMilliseconds
                        );
                    }

                    client.EndConnect(connectTask);
                    return DiagnosticResult.Pass(
                        step,
                        string.Format(
                            "Kết nối TCP thành công đến {0}:{1} ({2}ms).",
                            server,
                            port,
                            sw.ElapsedMilliseconds
                        ),
                        (int)sw.ElapsedMilliseconds
                    );
                }
            }
            catch (SocketException sockEx)
            {
                sw.Stop();
                string detail;
                string suggestion;

                switch (sockEx.SocketErrorCode)
                {
                    case SocketError.HostNotFound:
                        detail = "Không tìm thấy máy chủ '" + server + "'.";
                        suggestion =
                            "Kiểm tra lại tên máy chủ hoặc địa chỉ IP.\n"
                            + "Thử ping máy chủ từ cmd: ping "
                            + server;
                        break;

                    case SocketError.ConnectionRefused:
                        detail = "Máy chủ từ chối kết nối trên cổng " + port + ".";
                        suggestion =
                            "SQL Server có thể chưa bật hoặc đang dùng cổng khác.\n"
                            + "Mở SQL Server Configuration Manager → TCP/IP → kiểm tra port.";
                        break;

                    case SocketError.TimedOut:
                        detail = "Kết nối timeout đến " + server + ":" + port + ".";
                        suggestion =
                            "Máy chủ có thể tắt hoặc tường lửa chặn.\n"
                            + "Kiểm tra Windows Firewall → Inbound Rules.";
                        break;

                    default:
                        detail = "Lỗi mạng: " + sockEx.Message;
                        suggestion = "Kiểm tra kết nối mạng cơ bản.\n" + "Thử: ping " + server;
                        break;
                }

                return DiagnosticResult.Fail(step, detail, suggestion, (int)sw.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                sw.Stop();
                return DiagnosticResult.Fail(
                    step,
                    "Lỗi không xác định: " + ex.Message,
                    "Kiểm tra tường lửa và kết nối mạng.",
                    (int)sw.ElapsedMilliseconds
                );
            }
        }

        /// <summary>
        /// Bước 2: Phân tích quyền sở hữu danh tính SQL Server - Kiểm thử xem tài khoản cung cấp có truy cập nổi vào DB "master" không.
        /// Cô lập quy trình xác thực chứng chỉ khởi đầu từ quy trình đọc CSDL vật lý (DB existence).
        /// </summary>
        public static DiagnosticResult TestAuthentication(
            string server,
            int port,
            string username,
            string password,
            int timeoutSeconds = 5
        )
        {
            string step = "Xác thực SQL Server";
            var sw = System.Diagnostics.Stopwatch.StartNew();

            string connStr = string.Format(
                "Server={0},{1};Database=master;User Id={2};Password={3};"
                    + "TrustServerCertificate=True;Connection Timeout={4}",
                server,
                port,
                username,
                password,
                timeoutSeconds
            );

            try
            {
                using (var conn = new SqlConnection(connStr))
                {
                    conn.Open();
                    sw.Stop();
                    return DiagnosticResult.Pass(
                        step,
                        string.Format(
                            "Đăng nhập thành công với tài khoản '{0}' ({1}ms).",
                            username,
                            sw.ElapsedMilliseconds
                        ),
                        (int)sw.ElapsedMilliseconds
                    );
                }
            }
            catch (SqlException sqlEx)
            {
                sw.Stop();

                if (sqlEx.Number == 18456)
                {
                    return DiagnosticResult.Fail(
                        step,
                        "Đăng nhập thất bại cho tài khoản '" + username + "'.",
                        "Kiểm tra:\n"
                            + "  • Tên đăng nhập và mật khẩu có đúng không?\n"
                            + "  • SQL Server có bật 'SQL Server Authentication Mode' không?\n"
                            + "    (SSMS → Server Properties → Security → Both)\n"
                            + "  • Tài khoản 'sa' có bị khóa không?",
                        (int)sw.ElapsedMilliseconds
                    );
                }

                return DiagnosticResult.Fail(
                    step,
                    "Lỗi SQL: " + sqlEx.Message,
                    "Kiểm tra thông tin đăng nhập.",
                    (int)sw.ElapsedMilliseconds
                );
            }
            catch (Exception ex)
            {
                sw.Stop();
                return DiagnosticResult.Fail(
                    step,
                    "Lỗi: " + ex.Message,
                    "Kiểm tra cài đặt SQL Server.",
                    (int)sw.ElapsedMilliseconds
                );
            }
        }

        /// <summary>
        /// Bước 3: Phát hiện kho dữ liệu đích - Thực sự kiểm tra không gian lưu trữ đã khởi tạo chuẩn CSDL quy định.
        /// </summary>
        public static DiagnosticResult TestDatabaseExists(
            string server,
            int port,
            string username,
            string password,
            string database = "QuanLyNhanVien",
            int timeoutSeconds = 5
        )
        {
            string step = "Cơ sở dữ liệu";
            var sw = System.Diagnostics.Stopwatch.StartNew();

            string connStr = string.Format(
                "Server={0},{1};Database={2};User Id={3};Password={4};"
                    + "TrustServerCertificate=True;Connection Timeout={5}",
                server,
                port,
                database,
                username,
                password,
                timeoutSeconds
            );

            try
            {
                using (var conn = new SqlConnection(connStr))
                {
                    conn.Open();
                    sw.Stop();
                    return DiagnosticResult.Pass(
                        step,
                        string.Format(
                            "Database '{0}' tồn tại và truy cập được ({1}ms).",
                            database,
                            sw.ElapsedMilliseconds
                        ),
                        (int)sw.ElapsedMilliseconds
                    );
                }
            }
            catch (SqlException sqlEx)
            {
                sw.Stop();

                if (sqlEx.Number == 4060)
                {
                    return DiagnosticResult.Fail(
                        step,
                        "Database '" + database + "' không tồn tại.",
                        "Cần tạo database trước:\n"
                            + "  1. Mở SSMS hoặc sqlcmd\n"
                            + "  2. Chạy script CreateDatabase.sql\n"
                            + "  Hoặc liên hệ quản trị viên.",
                        (int)sw.ElapsedMilliseconds
                    );
                }

                return DiagnosticResult.Fail(
                    step,
                    "Lỗi truy cập database: " + sqlEx.Message,
                    "Kiểm tra quyền truy cập của tài khoản.",
                    (int)sw.ElapsedMilliseconds
                );
            }
            catch (Exception ex)
            {
                sw.Stop();
                return DiagnosticResult.Fail(
                    step,
                    "Lỗi: " + ex.Message,
                    "Kiểm tra cấu hình database.",
                    (int)sw.ElapsedMilliseconds
                );
            }
        }

        /// <summary>
        /// Bước 4: Kiểm kê tính hệ thống (Schema) xem bảng khởi tạo và cấp phép quyền sử dụng đã khả thi chưa.
        /// </summary>
        public static DiagnosticResult TestSchemaReady(
            string server,
            int port,
            string username,
            string password,
            string database = "QuanLyNhanVien",
            int timeoutSeconds = 5
        )
        {
            string step = "Cấu trúc bảng";
            var sw = System.Diagnostics.Stopwatch.StartNew();

            string connStr = string.Format(
                "Server={0},{1};Database={2};User Id={3};Password={4};"
                    + "TrustServerCertificate=True;Connection Timeout={5}",
                server,
                port,
                database,
                username,
                password,
                timeoutSeconds
            );

            try
            {
                using (var conn = new SqlConnection(connStr))
                {
                    conn.Open();
                    using (
                        var cmd = new SqlCommand(
                            "SELECT COUNT(*) FROM sys.tables WHERE name IN "
                                + "('TaiKhoan','BoPhan','NhanVien','BangLuong')",
                            conn
                        )
                    )
                    {
                        int count = (int)cmd.ExecuteScalar();
                        sw.Stop();

                        if (count >= 4)
                        {
                            return DiagnosticResult.Pass(
                                step,
                                string.Format(
                                    "Đã tìm thấy {0}/4 bảng cần thiết ({1}ms).",
                                    count,
                                    sw.ElapsedMilliseconds
                                ),
                                (int)sw.ElapsedMilliseconds
                            );
                        }

                        return DiagnosticResult.Fail(
                            step,
                            string.Format("Chỉ tìm thấy {0}/4 bảng cần thiết.", count),
                            "Cần chạy script khởi tạo database:\n"
                                + "  1. Mở SSMS hoặc sqlcmd\n"
                                + "  2. Chạy script CreateDatabase.sql\n"
                                + "  3. Sau đó chạy các migration script (002_*, 003_*, ...)",
                            (int)sw.ElapsedMilliseconds
                        );
                    }
                }
            }
            catch (Exception ex)
            {
                sw.Stop();
                return DiagnosticResult.Fail(
                    step,
                    "Lỗi kiểm tra bảng: " + ex.Message,
                    "Database có thể chưa được khởi tạo đúng cách.",
                    (int)sw.ElapsedMilliseconds
                );
            }
        }

        /// <summary>
        /// Gọi thủ tục cho chạy toàn tập 4 bài kiểm tra chẩn đoán lỗi liên tục.
        /// Ngưng kết nối (dừng quy trình lập tức) khi bất kỳ test nhỏ nào gãy đổ (Fail Fast).
        /// </summary>
        public static DiagnosticResult[] RunFullDiagnostic(
            string server,
            int port,
            string username,
            string password,
            string database
        )
        {
            var results = new DiagnosticResult[4];

            results[0] = TestTcpConnectivity(server, port);
            if (!results[0].Success)
                return new[] { results[0] };

            results[1] = TestAuthentication(server, port, username, password);
            if (!results[1].Success)
                return new[] { results[0], results[1] };

            results[2] = TestDatabaseExists(server, port, username, password, database);
            if (!results[2].Success)
                return new[] { results[0], results[1], results[2] };

            results[3] = TestSchemaReady(server, port, username, password, database);
            return results;
        }

        /// <summary>
        /// Kết nối nhanh: Lập tức ép tiến trình kết nối mở khóa thông qua chuỗi url thiết đặt cho trước.
        /// Trả về rỗng theo hàm ẩn hoặc mang dữ liệu log ngoại lệ nếu ném lỗi ra khối try/catch.
        /// </summary>
        public static Exception QuickTest(string connectionString, int timeoutSeconds = 5)
        {
            try
            {
                // Tiêm thiết lập timeout thủ công trong trường hợp không được tự động cung cấp
                var builder = new SqlConnectionStringBuilder(connectionString);
                builder.ConnectTimeout = timeoutSeconds;

                using (var conn = new SqlConnection(builder.ToString()))
                {
                    conn.Open();
                    using (var cmd = new SqlCommand("SELECT 1", conn))
                    {
                        cmd.ExecuteScalar();
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                return ex;
            }
        }

        /// <summary>
        /// Lắp ghép và biên dịch thành cấu trúc ConnectionString hợp quy cho CSDL từ từng thông số truyền nhỏ lẻ.
        /// </summary>
        public static string BuildConnectionString(
            string server,
            int port,
            string username,
            string password,
            string database = "QuanLyNhanVien"
        )
        {
            return string.Format(
                "Server={0},{1};Database={2};User Id={3};Password={4};TrustServerCertificate=True",
                server,
                port,
                database,
                username,
                password
            );
        }
    }
}
