using System;
using System.Threading;
using System.Windows.Forms;

namespace QuanLyNhanVien.Infrastructure
{
    /// <summary>
    /// Trình xử lý ngoại lệ toàn cục — cài đặt các trình xử lý bao quát cho
    /// cả các ngoại lệ không được xử lý trên luồng UI và ngoài luồng UI.
    ///
    /// Mục đích:
    /// 1. Ngăn chặn sự cố ứng dụng bị đóng băng hoặc thoát đột ngột mà không báo trước
    /// 2. Ghi log TẤT CẢ các ngoại lệ chưa được bắt (unhandled) qua AppLogger (vào File + DB)
    /// 3. Hiển thị thông báo thân thiện với người dùng dựa trên mức độ nghiêm trọng
    /// 4. Đối với các lỗi nghiêm trọng (Critical): đề xuất khởi động lại ứng dụng
    ///
    /// Phải được cài đặt NGAY TRƯỚC Application.Run() trong Program.Main().
    /// </summary>
    public static class GlobalExceptionHandler
    {
        /// <summary>
        /// Cài đặt trình xử lý lỗi toàn cục. Chỉ gọi một lần lúc khởi động ứng dụng.
        /// </summary>
        public static void Install()
        {
            // Xử lý lỗi trên luồng UI (Vòng lặp thông báo WinForms)
            Application.ThreadException += OnThreadException;
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);

            // Xử lý lỗi ngoài luồng UI (luồng ngầm, bộ dọn rác - finalizers)
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
        }

        /// <summary>
        /// Xử lý các ngoại lệ trên luồng UI. Đây là các lỗi có thể phục hồi được —
        /// ứng dụng có thể tiếp tục hoạt động sau khi ta bắt lỗi này.
        /// </summary>
        private static void OnThreadException(object sender, ThreadExceptionEventArgs e)
        {
            HandleException(e.Exception, isFatal: false);
        }

        /// <summary>
        /// Xử lý các ngoại lệ không thuộc luồng UI. Các lỗi này thường nghiêm trọng —
        /// bộ thực thi CLR sẽ tự động ngắt ứng dụng sau khi trình xử lý này kết thúc.
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
                // Lỗi không thuộc luồng Exception (rất hiếm trong C#)
                AppLogger.Critical(
                    "GlobalExceptionHandler",
                    "Lỗi nghiêm trọng không xác định: " + e.ExceptionObject
                );
            }
        }

        /// <summary>
        /// Hàm xử lý cốt lõi: Ghi log → Phân loại lỗi → Hiện hộp thoại cho người dùng.
        /// </summary>
        private static void HandleException(Exception ex, bool isFatal)
        {
            // ── Bước 1: Phân loại mức độ nghiêm trọng của Exception ──
            LogLevel level;
            string userTitle;
            string userMessage;

            if (ex is OutOfMemoryException || ex is StackOverflowException)
            {
                level = LogLevel.Critical;
                userTitle = "Lỗi Nghiêm Trọng";
                userMessage =
                    "Ứng dụng gặp lỗi bộ nhớ nghiêm trọng.\n" + "Vui lòng khởi động lại ứng dụng.";
            }
            else if (ex is System.Data.SqlClient.SqlException sqlEx)
            {
                level = LogLevel.Error;
                userTitle = "Lỗi Cơ Sở Dữ Liệu";
                userMessage = ClassifySqlError(sqlEx);
            }
            else if (ex is System.Net.Sockets.SocketException || ex is TimeoutException)
            {
                level = LogLevel.Error;
                userTitle = "Lỗi Kết Nối";
                userMessage =
                    "Không thể kết nối đến máy chủ cơ sở dữ liệu.\n"
                    + "Vui lòng kiểm tra kết nối mạng và thử lại.\n\n"
                    + "Chi tiết: "
                    + ex.Message;
            }
            else if (ex is UnauthorizedAccessException)
            {
                level = LogLevel.Warning;
                userTitle = "Quyền Truy Cập";
                userMessage =
                    "Không có quyền truy cập tài nguyên yêu cầu.\n" + "Chi tiết: " + ex.Message;
            }
            else if (ex is InvalidOperationException)
            {
                level = LogLevel.Error;
                userTitle = "Lỗi Thao Tác";
                userMessage = "Thao tác không hợp lệ đã xảy ra.\n" + "Chi tiết: " + ex.Message;
            }
            else
            {
                level = isFatal ? LogLevel.Critical : LogLevel.Error;
                userTitle = isFatal ? "Lỗi Nghiêm Trọng" : "Lỗi Ứng Dụng";
                userMessage = "Đã xảy ra lỗi không mong muốn.\n\n" + "Chi tiết: " + ex.Message;
            }

            // ── Bước 2: Ghi dữ liệu log vào File và Database ──
            string source =
                ex.TargetSite != null
                    ? ex.TargetSite.DeclaringType?.FullName + "." + ex.TargetSite.Name
                    : "Unknown";
            AppLogger.Log(level, source, ex.Message, ex);

            // ── Bước 3: Hiển thị hộp thoại thân thiện ──
            try
            {
                var icon = level == LogLevel.Critical ? MessageBoxIcon.Stop : MessageBoxIcon.Error;

                if (isFatal)
                {
                    MessageBox.Show(
                        userMessage + "\n\nỨng dụng sẽ đóng lại.",
                        userTitle,
                        MessageBoxButtons.OK,
                        icon
                    );
                }
                else
                {
                    MessageBox.Show(
                        userMessage
                            + "\n\nNếu lỗi tiếp tục lặp lại, vui lòng liên hệ bộ phận kỹ thuật.",
                        userTitle,
                        MessageBoxButtons.OK,
                        icon
                    );
                }
            }
            catch
            {
                // Trong trường hợp ngay cả hộp thoại MessageBox cũng bị lỗi
                // (rất hiếm tốn bộ nhớ/GPU), ít nhất là log cũng đã được lưu.
            }
        }

        /// <summary>
        /// Dịch mã lỗi SQL Server sang ngôn ngữ tiếng Việt dễ hiểu.
        /// Bao gồm các mã lỗi phổ biến nhất trong môi trường thực tế (production).
        /// </summary>
        private static string ClassifySqlError(System.Data.SqlClient.SqlException sqlEx)
        {
            switch (sqlEx.Number)
            {
                // Lỗi mất kết nối / Kết nối mạng
                case -1:
                case 2:
                case 53:
                    return "Không thể kết nối đến SQL Server.\n"
                        + "Kiểm tra:\n"
                        + "  • Tên máy chủ / IP có đúng không?\n"
                        + "  • SQL Server đã bật và chạy chưa?\n"
                        + "  • Tường lửa có chặn cổng 1433 không?";

                // Lỗi đăng nhập thất bại
                case 18456:
                    return "Đăng nhập SQL Server thất bại.\n"
                        + "Kiểm tra tên đăng nhập và mật khẩu trong cấu hình kết nối.";

                // Database không tồn tại
                case 4060:
                    return "Cơ sở dữ liệu 'QuanLyNhanVien' không tồn tại.\n"
                        + "Cần chạy script tạo database trước khi sử dụng.";

                // Không đủ quyền bảo mật
                case 229:
                case 230:
                    return "Không có quyền thực hiện thao tác này trên cơ sở dữ liệu.\n"
                        + "Liên hệ quản trị viên để cấp quyền.";

                // Xung đột Deadlock
                case 1205:
                    return "Xung đột khóa dữ liệu (deadlock).\n" + "Vui lòng thử lại thao tác.";

                // Quá thời gian Time Out
                case -2:
                    return "Truy vấn bị timeout.\n"
                        + "Máy chủ đang quá tải hoặc truy vấn quá nặng.\n"
                        + "Vui lòng thử lại sau.";

                // Vi phạm ràng buộc Khóa / Unique
                case 2627:
                case 2601:
                    return "Dữ liệu bị trùng lặp — vi phạm ràng buộc duy nhất.\n"
                        + "Kiểm tra lại thông tin nhập vào.";

                // Vi phạm Khóa Ngoại ForeignKey
                case 547:
                    return "Vi phạm ràng buộc dữ liệu liên quan.\n"
                        + "Không thể xóa hoặc sửa vì có dữ liệu phụ thuộc.";

                default:
                    return "Lỗi SQL Server (mã " + sqlEx.Number + "):\n" + sqlEx.Message;
            }
        }
    }
}
