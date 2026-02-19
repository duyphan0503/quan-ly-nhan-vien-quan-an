using System;
using System.Threading;
using System.Windows.Forms;

namespace QuanLyNhanVien.Infrastructure
{
    /// <summary>
    /// Global exception handler — installs top-level catch-all handlers for
    /// both UI-thread and non-UI-thread unhandled exceptions.
    /// 
    /// Purpose:
    /// 1. Prevents the app from silently crashing
    /// 2. Logs ALL unhandled exceptions via AppLogger (file + DB)
    /// 3. Shows a user-friendly error dialog with severity-appropriate message
    /// 4. For Critical errors: offers to restart the application
    /// 
    /// Must be installed BEFORE Application.Run() in Program.Main().
    /// </summary>
    public static class GlobalExceptionHandler
    {
        /// <summary>
        /// Installs global exception handlers. Call once at app startup.
        /// </summary>
        public static void Install()
        {
            // UI thread exceptions (WinForms message loop)
            Application.ThreadException += OnThreadException;
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);

            // Non-UI thread exceptions (background threads, finalizers)
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
        }

        /// <summary>
        /// Handles exceptions on the UI thread. These are recoverable — the
        /// message loop continues after we handle them.
        /// </summary>
        private static void OnThreadException(object sender, ThreadExceptionEventArgs e)
        {
            HandleException(e.Exception, isFatal: false);
        }

        /// <summary>
        /// Handles exceptions on non-UI threads. These are generally fatal —
        /// the CLR will terminate after this handler returns.
        /// </summary>
        private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = e.ExceptionObject as Exception;
            if (ex != null)
            {
                HandleException(ex, isFatal: e.IsTerminating);
            }
            else
            {
                // Non-Exception throwable (very rare in C#)
                AppLogger.Critical("GlobalExceptionHandler",
                    "Non-Exception unhandled error: " + e.ExceptionObject);
            }
        }

        /// <summary>
        /// Core handler: logs → classifies → shows user dialog.
        /// </summary>
        private static void HandleException(Exception ex, bool isFatal)
        {
            // ── Step 1: Classify severity based on exception type ──
            LogLevel level;
            string userTitle;
            string userMessage;

            if (ex is OutOfMemoryException || ex is StackOverflowException)
            {
                level = LogLevel.Critical;
                userTitle = "Lỗi Nghiêm Trọng";
                userMessage = "Ứng dụng gặp lỗi bộ nhớ nghiêm trọng.\n"
                    + "Vui lòng khởi động lại ứng dụng.";
            }
            else if (ex is System.Data.SqlClient.SqlException sqlEx)
            {
                level = LogLevel.Error;
                userTitle = "Lỗi Cơ Sở Dữ Liệu";
                userMessage = ClassifySqlError(sqlEx);
            }
            else if (ex is System.Net.Sockets.SocketException
                  || ex is TimeoutException)
            {
                level = LogLevel.Error;
                userTitle = "Lỗi Kết Nối";
                userMessage = "Không thể kết nối đến máy chủ cơ sở dữ liệu.\n"
                    + "Vui lòng kiểm tra kết nối mạng và thử lại.\n\n"
                    + "Chi tiết: " + ex.Message;
            }
            else if (ex is UnauthorizedAccessException)
            {
                level = LogLevel.Warning;
                userTitle = "Quyền Truy Cập";
                userMessage = "Không có quyền truy cập tài nguyên yêu cầu.\n"
                    + "Chi tiết: " + ex.Message;
            }
            else if (ex is InvalidOperationException)
            {
                level = LogLevel.Error;
                userTitle = "Lỗi Thao Tác";
                userMessage = "Thao tác không hợp lệ đã xảy ra.\n"
                    + "Chi tiết: " + ex.Message;
            }
            else
            {
                level = isFatal ? LogLevel.Critical : LogLevel.Error;
                userTitle = isFatal ? "Lỗi Nghiêm Trọng" : "Lỗi Ứng Dụng";
                userMessage = "Đã xảy ra lỗi không mong muốn.\n\n"
                    + "Chi tiết: " + ex.Message;
            }

            // ── Step 2: Log to file + DB ──
            string source = ex.TargetSite != null
                ? ex.TargetSite.DeclaringType?.FullName + "." + ex.TargetSite.Name
                : "Unknown";
            AppLogger.Log(level, source, ex.Message, ex);

            // ── Step 3: Show user-friendly dialog ──
            try
            {
                var icon = level == LogLevel.Critical
                    ? MessageBoxIcon.Stop
                    : MessageBoxIcon.Error;

                if (isFatal)
                {
                    MessageBox.Show(
                        userMessage + "\n\nỨng dụng sẽ đóng lại.",
                        userTitle,
                        MessageBoxButtons.OK, icon);
                }
                else
                {
                    MessageBox.Show(
                        userMessage
                        + "\n\nNếu lỗi tiếp tục lặp lại, vui lòng liên hệ bộ phận kỹ thuật.",
                        userTitle,
                        MessageBoxButtons.OK, icon);
                }
            }
            catch
            {
                // If even the message box fails (possible in extreme scenarios),
                // at least we've logged it.
            }
        }

        /// <summary>
        /// Classifies SQL Server error codes into user-friendly Vietnamese messages.
        /// These are the most common error numbers encountered in production.
        /// </summary>
        private static string ClassifySqlError(System.Data.SqlClient.SqlException sqlEx)
        {
            switch (sqlEx.Number)
            {
                // Connection errors
                case -1:
                case 2:
                case 53:
                    return "Không thể kết nối đến SQL Server.\n"
                         + "Kiểm tra:\n"
                         + "  • Tên máy chủ / IP có đúng không?\n"
                         + "  • SQL Server đã bật và chạy chưa?\n"
                         + "  • Tường lửa có chặn cổng 1433 không?";

                // Login failed
                case 18456:
                    return "Đăng nhập SQL Server thất bại.\n"
                         + "Kiểm tra tên đăng nhập và mật khẩu trong cấu hình kết nối.";

                // Database doesn't exist
                case 4060:
                    return "Cơ sở dữ liệu 'QuanLyNhanVien' không tồn tại.\n"
                         + "Cần chạy script tạo database trước khi sử dụng.";

                // Permission denied
                case 229:
                case 230:
                    return "Không có quyền thực hiện thao tác này trên cơ sở dữ liệu.\n"
                         + "Liên hệ quản trị viên để cấp quyền.";

                // Deadlock
                case 1205:
                    return "Xung đột khóa dữ liệu (deadlock).\n"
                         + "Vui lòng thử lại thao tác.";

                // Timeout
                case -2:
                    return "Truy vấn bị timeout.\n"
                         + "Máy chủ đang quá tải hoặc truy vấn quá nặng.\n"
                         + "Vui lòng thử lại sau.";

                // Constraint violation
                case 2627:
                case 2601:
                    return "Dữ liệu bị trùng lặp — vi phạm ràng buộc duy nhất.\n"
                         + "Kiểm tra lại thông tin nhập vào.";

                // FK violation
                case 547:
                    return "Vi phạm ràng buộc dữ liệu liên quan.\n"
                         + "Không thể xóa hoặc sửa vì có dữ liệu phụ thuộc.";

                default:
                    return "Lỗi SQL Server (mã " + sqlEx.Number + "):\n" + sqlEx.Message;
            }
        }
    }
}
