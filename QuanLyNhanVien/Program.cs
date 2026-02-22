using System;
using System.Windows.Forms;
using QuanLyNhanVien.DataAccess;
using QuanLyNhanVien.Forms;
using QuanLyNhanVien.Infrastructure;

namespace QuanLyNhanVien
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // ── Bước 1: Khởi động bộ xử lý đánh chặn ngoại lệ cục bộ (global exception handlers) ĐẦU TIÊN ──
            // Thao tác này phải diễn ra trước BẤT KỲ đoạn mã chạy nền nào khác, đảm bảo các
            // lỗi phát sinh sớm sẽ bị bắt lại, đính vào log và gửi cảnh báo nguyên bản.
            GlobalExceptionHandler.Install();
            AppLogger.Info("Program", "Ứng dụng khởi động.");

            // ── Bước 2: Test thăm dò CSDL tĩnh (database connection) ──
            // Nếu đánh giá kết nối thất bại, mở tự động trình Wizard Kết Nối
            // nhằm hỗ trợ client thiết lập đường truyền tới điểm máy chủ của SQL Server.
            bool connectionReady = false;

            try
            {
                connectionReady = DatabaseHelper.TestConnection(timeoutSeconds: 3);
            }
            catch
            {
                // Cấu hình không có hoặc bị sai định dạng — wizard sẽ đảm nhận việc khôi phục nó
                connectionReady = false;
            }

            if (!connectionReady)
            {
                AppLogger.Warning(
                    "Program",
                    "Kết nối CSDL thất bại — khởi chạy Connection Wizard."
                );

                using (var wizard = new FormConnectionWizard())
                {
                    var result = wizard.ShowDialog();

                    if (result != DialogResult.OK || !wizard.ConfigurationSaved)
                    {
                        AppLogger.Info(
                            "Program",
                            "Người dùng thoát Connection Wizard — đóng ứng dụng."
                        );
                        return; // Ngắt thoát khỏi ứng dụng hoàn toàn
                    }

                    // Việc cài qua Wizard hoàn tất — tải mới chuỗi liên kết
                    DatabaseHelper.RefreshConnectionString();

                    // Xác minh lại kết nối mới cấu hình liệu đã truy cập hợp lệ chưa
                    if (!DatabaseHelper.TestConnection(timeoutSeconds: 5))
                    {
                        AppLogger.Error("Program", "Kết nối vẫn thất bại sau khi wizard hoàn tất.");
                        MessageBox.Show(
                            "Cấu hình đã được lưu nhưng vẫn không thể kết nối.\n"
                                + "Vui lòng kiểm tra lại SQL Server và khởi động lại ứng dụng.",
                            "Cảnh Báo",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning
                        );
                        return;
                    }
                }
            }

            AppLogger.Info("Program", "Kết nối CSDL thành công — hiển thị FormLogin.");

            // ── Bước 3: Cho chạy mẫu Login ──
            Application.Run(new FormLogin());
        }
    }
}
