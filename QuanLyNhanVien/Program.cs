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

            // ── Step 1: Install global exception handlers FIRST ──
            // This must happen before ANY other code runs, so that even
            // startup errors are caught, logged, and shown to the user.
            GlobalExceptionHandler.Install();
            AppLogger.Info("Program", "Ứng dụng khởi động.");

            // ── Step 2: Test database connection ──
            // If the connection fails, launch the Connection Wizard
            // so the client can configure their SQL Server connection.
            bool connectionReady = false;

            try
            {
                connectionReady = DatabaseHelper.TestConnection(timeoutSeconds: 3);
            }
            catch
            {
                // Config may be missing or malformed — wizard will handle it
                connectionReady = false;
            }

            if (!connectionReady)
            {
                AppLogger.Warning("Program",
                    "Kết nối CSDL thất bại — khởi chạy Connection Wizard.");

                using (var wizard = new FormConnectionWizard())
                {
                    var result = wizard.ShowDialog();

                    if (result != DialogResult.OK || !wizard.ConfigurationSaved)
                    {
                        AppLogger.Info("Program",
                            "Người dùng thoát Connection Wizard — đóng ứng dụng.");
                        return; // Exit the application
                    }

                    // Wizard saved successfully — refresh the connection string
                    DatabaseHelper.RefreshConnectionString();

                    // Verify the new connection actually works
                    if (!DatabaseHelper.TestConnection(timeoutSeconds: 5))
                    {
                        AppLogger.Error("Program",
                            "Kết nối vẫn thất bại sau khi wizard hoàn tất.");
                        MessageBox.Show(
                            "Cấu hình đã được lưu nhưng vẫn không thể kết nối.\n"
                            + "Vui lòng kiểm tra lại SQL Server và khởi động lại ứng dụng.",
                            "Cảnh Báo",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }
            }

            AppLogger.Info("Program", "Kết nối CSDL thành công — hiển thị FormLogin.");

            // ── Step 3: Launch the login form ──
            Application.Run(new FormLogin());
        }
    }
}
