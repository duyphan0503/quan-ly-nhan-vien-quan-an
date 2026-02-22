using System;
using System.Configuration;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using QuanLyNhanVien.Infrastructure;

namespace QuanLyNhanVien.Forms
{
    /// <summary>
    /// Wizard cài đặt cấu hình Kết nối trực tiếp — đưa người dùng trải qua quy trình thiết lập
    /// tham số SQL Server ở lần khởi chạy đầu tiên hay ngay khi gặp rớt kết nối mạng.
    ///
    /// Chia thành 4 bước (Step) logic đánh giá cụ thể:
    ///   Bước 1: Máy chủ host + cổng (port)
    ///   Bước 2: Chìa khóa Xác Thực (Credentials)
    ///   Bước 3: Tự động chạy chẩn đoán lỗi (TCP → Auth → DB → Schema)
    ///   Bước 4: Lưu File Cấu Hình App.Config
    ///
    /// Mẫu biểu điều hướng thân thiện này thiết kế đặc biệt tập trung cho dân công sở CNTT hỗ trợ (IT staff) cài đặt
    /// sản phẩm tận nơi (client sites) tại thiết bị cá nhân để quản trị trực tiếp vùng nhớ ảo.
    /// </summary>
    public partial class FormConnectionWizard : Form
    {
        // ── Trạng Thái Máy ──
        private int _currentStep = 1;
        private const int TOTAL_STEPS = 4;
        private bool _connectionSucceeded;
        private string _finalConnectionString;

        /// <summary>
        /// True nếu toàn bộ chu trình thông qua tuyệt đối và lưu thành công.
        /// </summary>
        public bool ConfigurationSaved { get; private set; }

        public FormConnectionWizard()
        {
            InitializeComponent();
            ApplyTheme();
            WireEvents();
            ShowStep(1);
            PreFillFromConfig();
        }

        private void ApplyTheme()
        {
            this.BackColor = AppColors.Crust;
            pnlCard.BackColor = AppColors.Base;

            lblLogo.Font = AppFonts.Create(32);
            lblLogo.ForeColor = AppColors.Blue;
            lblTitle.Font = AppFonts.Create(13, System.Drawing.FontStyle.Bold);
            lblTitle.ForeColor = AppColors.Blue;
            lblStepTitle.Font = AppFonts.BodyBold;
            lblStepTitle.ForeColor = AppColors.Text;
            lblStepDesc.Font = AppFonts.Tiny;
            lblStepDesc.ForeColor = AppColors.Overlay;

            // Bước 1
            lblServer.Font = AppFonts.Small;
            lblServer.ForeColor = AppColors.SubText;
            txtServer.Font = AppFonts.Body;
            txtServer.BackColor = AppColors.InputBg;
            lblPort.Font = AppFonts.Small;
            lblPort.ForeColor = AppColors.SubText;
            txtPort.Font = AppFonts.Body;
            txtPort.BackColor = AppColors.InputBg;
            lblTip1.Font = AppFonts.Tiny;
            lblTip1.ForeColor = AppColors.Overlay;

            // Bước 2
            lblUsername.Font = AppFonts.Small;
            lblUsername.ForeColor = AppColors.SubText;
            txtUsername.Font = AppFonts.Body;
            txtUsername.BackColor = AppColors.InputBg;
            lblPassword.Font = AppFonts.Small;
            lblPassword.ForeColor = AppColors.SubText;
            txtPassword.Font = AppFonts.Body;
            txtPassword.BackColor = AppColors.InputBg;
            lblDatabase.Font = AppFonts.Small;
            lblDatabase.ForeColor = AppColors.SubText;
            txtDatabase.Font = AppFonts.Body;
            txtDatabase.BackColor = AppColors.InputBg;
            lblTip2.Font = AppFonts.Tiny;
            lblTip2.ForeColor = AppColors.Overlay;

            // Bước 3
            rtbDiagnostic.BackColor = AppColors.Mantle;
            rtbDiagnostic.ForeColor = AppColors.Text;
            rtbDiagnostic.Font = AppFonts.Small;

            // Bước 4
            lblResult.Font = AppFonts.Body;
            lblResult.ForeColor = AppColors.Text;

            // Hàng phím nhấn (Buttons)
            btnBack.Font = AppFonts.SmallBold;
            btnBack.ForeColor = AppColors.Text;
            btnBack.IdleColor = AppColors.Surface1;
            btnBack.HoverColor = AppColors.Surface2;
            btnBack.PressColor = AppColors.Surface0;

            btnCancel.Font = AppFonts.SmallBold;
            btnCancel.ForeColor = AppColors.Crust;
            btnCancel.IdleColor = AppColors.Red;
            btnCancel.HoverColor = AppColors.Lighten(AppColors.Red);
            btnCancel.PressColor = AppColors.Darken(AppColors.Red);

            btnNext.Font = AppFonts.SmallBold;
            btnNext.ForeColor = AppColors.Crust;
            btnNext.IdleColor = AppColors.Blue;
            btnNext.HoverColor = AppColors.Lighten(AppColors.Blue);
            btnNext.PressColor = AppColors.Darken(AppColors.Blue);

            // Biểu đồ mốc thông báo (Step indicator)
            lblStepIndicator.Font = AppFonts.Tiny;
            lblStepIndicator.ForeColor = AppColors.Overlay;
        }

        private void WireEvents()
        {
            btnNext.Click += BtnNext_Click;
            btnBack.Click += BtnBack_Click;
            btnCancel.Click += BtnCancel_Click;
            pnlCard.Paint += (s, e) =>
            {
                var g = e.Graphics;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                var rect = new Rectangle(0, 0, pnlCard.Width - 1, pnlCard.Height - 1);
                using (var pen = new Pen(Color.FromArgb(30, 166, 227, 161), 1f))
                {
                    int r = 14,
                        d = r * 2;
                    var path = new System.Drawing.Drawing2D.GraphicsPath();
                    path.AddArc(rect.X, rect.Y, d, d, 180, 90);
                    path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
                    path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
                    path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
                    path.CloseFigure();
                    g.DrawPath(pen, path);
                    path.Dispose();
                }
            };
        }

        // ── Tự dán dữ liệu (Pre-fill) lấy cấu hình gốc App.Config nếu có ──
        private void PreFillFromConfig()
        {
            try
            {
                var cs = ConfigurationManager.ConnectionStrings["QuanLyNhanVien"];
                if (cs != null && !string.IsNullOrEmpty(cs.ConnectionString))
                {
                    var builder = new System.Data.SqlClient.SqlConnectionStringBuilder(
                        cs.ConnectionString
                    );
                    string dataSource = builder.DataSource ?? "";

                    // Tách thành máy chủ (server) và cổng (port) cắm từ Data Source
                    if (dataSource.Contains(","))
                    {
                        var parts = dataSource.Split(',');
                        txtServer.Text = parts[0].Trim();
                        txtPort.Text = parts[1].Trim();
                    }
                    else
                    {
                        txtServer.Text = dataSource;
                    }

                    txtUsername.Text = builder.UserID ?? "";
                    txtPassword.Text = builder.Password ?? "";
                    txtDatabase.Text = !string.IsNullOrEmpty(builder.InitialCatalog)
                        ? builder.InitialCatalog
                        : "QuanLyNhanVien";
                }
            }
            catch
            {
                // Thất bại trong đọc phân tích file — người dùng cần thao tác bổ sung thủ công bằng tay
            }
        }

        // ── Điều Hướng Chuyển Cảnh Màn Hình ──
        private void ShowStep(int step)
        {
            _currentStep = step;

            pnlStep1.Visible = step == 1;
            pnlStep2.Visible = step == 2;
            pnlStep3.Visible = step == 3;
            pnlStep4.Visible = step == 4;

            btnBack.Enabled = step > 1;
            btnBack.Visible = step > 1 && step < 4;

            switch (step)
            {
                case 1:
                    lblStepTitle.Text = "Bước 1: Máy Chủ SQL Server";
                    lblStepDesc.Text = "Nhập tên máy chủ hoặc địa chỉ IP của SQL Server.";
                    btnNext.Text = "TIẾP TỤC →";
                    break;
                case 2:
                    lblStepTitle.Text = "Bước 2: Thông Tin Đăng Nhập";
                    lblStepDesc.Text = "Nhập tài khoản đăng nhập SQL Server.";
                    btnNext.Text = "KIỂM TRA →";
                    break;
                case 3:
                    lblStepTitle.Text = "Bước 3: Kiểm Tra Kết Nối";
                    lblStepDesc.Text = "Đang kiểm tra từng bước...";
                    btnNext.Text = "THỬ LẠI";
                    btnNext.Visible = false;
                    btnBack.Visible = false;
                    RunDiagnosticsAsync();
                    break;
                case 4:
                    lblStepTitle.Text = _connectionSucceeded
                        ? "Kết Nối Thành Công!"
                        : "Kết Nối Thất Bại";
                    lblStepDesc.Text = _connectionSucceeded
                        ? "Nhấn 'LƯU CẤU HÌNH' để lưu và bắt đầu sử dụng."
                        : "Vui lòng quay lại kiểm tra thông tin.";
                    btnNext.Visible = true;
                    btnBack.Visible = true;
                    btnNext.Text = _connectionSucceeded ? "LƯU CẤU HÌNH ✓" : "← QUAY LẠI";
                    break;
            }

            lblStepIndicator.Text = string.Format("Bước {0}/{1}", step, TOTAL_STEPS);
        }

        // ── Kích Hoạt Khởi Chạy Thuốc Thử (Diagnostic) ──
        private void RunDiagnosticsAsync()
        {
            rtbDiagnostic.Clear();
            pbDiagnostic.Value = 0;
            _connectionSucceeded = false;

            string server = txtServer.Text.Trim();
            int port;
            if (!int.TryParse(txtPort.Text.Trim(), out port))
                port = 1433;
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;
            string database = txtDatabase.Text.Trim();

            // Đóng hộp luồng ngầm giữ cho đồ hoạ cửa sổ UI mượt mà liên tục đáp ứng chuột
            var bgThread = new Thread(() =>
            {
                try
                {
                    // Cập nhật lên màn hình theo từng bước hiển thị
                    AppendDiagnostic("🔍 Bắt đầu kiểm tra kết nối...\n", Color.White);
                    UpdateProgress(5);

                    // 1. TCP
                    AppendDiagnostic("\n━━━ 1. Kiểm tra kết nối mạng (TCP) ━━━\n", AppColors.Blue);
                    var tcpResult = ConnectionDiagnostics.TestTcpConnectivity(server, port);
                    ShowDiagnosticResult(tcpResult);
                    UpdateProgress(25);
                    if (!tcpResult.Success)
                    {
                        FinishDiagnostic(false);
                        return;
                    }

                    // 2. Auth
                    AppendDiagnostic("\n━━━ 2. Kiểm tra đăng nhập ━━━\n", AppColors.Blue);
                    var authResult = ConnectionDiagnostics.TestAuthentication(
                        server,
                        port,
                        username,
                        password
                    );
                    ShowDiagnosticResult(authResult);
                    UpdateProgress(50);
                    if (!authResult.Success)
                    {
                        FinishDiagnostic(false);
                        return;
                    }

                    // 3. Database
                    AppendDiagnostic("\n━━━ 3. Kiểm tra cơ sở dữ liệu ━━━\n", AppColors.Blue);
                    var dbResult = ConnectionDiagnostics.TestDatabaseExists(
                        server,
                        port,
                        username,
                        password,
                        database
                    );
                    ShowDiagnosticResult(dbResult);
                    UpdateProgress(75);
                    if (!dbResult.Success)
                    {
                        FinishDiagnostic(false);
                        return;
                    }

                    // 4. Schema
                    AppendDiagnostic("\n━━━ 4. Kiểm tra cấu trúc bảng ━━━\n", AppColors.Blue);
                    var schemaResult = ConnectionDiagnostics.TestSchemaReady(
                        server,
                        port,
                        username,
                        password,
                        database
                    );
                    ShowDiagnosticResult(schemaResult);
                    UpdateProgress(100);

                    if (schemaResult.Success)
                    {
                        _finalConnectionString = ConnectionDiagnostics.BuildConnectionString(
                            server,
                            port,
                            username,
                            password,
                            database
                        );
                        AppendDiagnostic("\n\nTất cả kiểm tra đều thành công!\n", AppColors.Green);
                        FinishDiagnostic(true);
                    }
                    else
                    {
                        // Cơ sở cấu hình (Schema) chưa tải thành công nhưng — vẫn cho lưu lại trước (chạy file sinh sau)
                        _finalConnectionString = ConnectionDiagnostics.BuildConnectionString(
                            server,
                            port,
                            username,
                            password,
                            database
                        );
                        AppendDiagnostic(
                            "\n\nKết nối thành công nhưng cơ sở dữ liệu chưa sẵn sàng.\n"
                                + "Bạn vẫn có thể lưu cấu hình và khởi tạo database sau.\n",
                            AppColors.Yellow
                        );
                        FinishDiagnostic(true);
                    }
                }
                catch (Exception ex)
                {
                    AppendDiagnostic("\n\nLỗi không xác định: " + ex.Message + "\n", AppColors.Red);
                    FinishDiagnostic(false);
                }
            });

            bgThread.IsBackground = true;
            bgThread.Start();
        }

        private void ShowDiagnosticResult(DiagnosticResult result)
        {
            if (result.Success)
            {
                AppendDiagnostic("  " + result.Message + "\n", AppColors.Green);
            }
            else
            {
                AppendDiagnostic("  " + result.Message + "\n", AppColors.Red);
                if (!string.IsNullOrEmpty(result.Suggestion))
                {
                    AppendDiagnostic("\n  💡 Gợi ý khắc phục:\n", AppColors.Yellow);
                    AppendDiagnostic(
                        "  " + result.Suggestion.Replace("\n", "\n  ") + "\n",
                        AppColors.SubText
                    );
                }
            }
        }

        // ── Điều hướng luồng Safe luồng Thread-safe trên UI ──
        private void AppendDiagnostic(string text, Color color)
        {
            if (rtbDiagnostic.InvokeRequired)
            {
                rtbDiagnostic.Invoke((Action)(() => AppendDiagnostic(text, color)));
                return;
            }
            rtbDiagnostic.SelectionStart = rtbDiagnostic.TextLength;
            rtbDiagnostic.SelectionLength = 0;
            rtbDiagnostic.SelectionColor = color;
            rtbDiagnostic.AppendText(text);
            rtbDiagnostic.ScrollToCaret();
        }

        private void UpdateProgress(int percent)
        {
            if (pbDiagnostic.InvokeRequired)
            {
                pbDiagnostic.Invoke((Action)(() => UpdateProgress(percent)));
                return;
            }
            pbDiagnostic.Value = percent;
        }

        private void FinishDiagnostic(bool success)
        {
            _connectionSucceeded = success;
            if (this.InvokeRequired)
            {
                this.Invoke((Action)(() => FinishDiagnostic_UI(success)));
                return;
            }
            FinishDiagnostic_UI(success);
        }

        private void FinishDiagnostic_UI(bool success)
        {
            // Kích hoạt bảng điều hướng trở lại màn hình
            btnNext.Visible = true;
            btnBack.Visible = true;

            if (success)
            {
                btnNext.Text = "TIẾP TỤC →";
                btnNext.IdleColor = AppColors.Green;
            }
            else
            {
                btnNext.Text = "THỬ LẠI ↻";
                btnNext.IdleColor = AppColors.Yellow;
            }
            btnNext.Invalidate();
        }

        // ── Lưu cấu hình gốc App.Config ──
        private bool SaveConfiguration()
        {
            try
            {
                // Tác động trực tiếp đến bộ API ghi trên nền XML của thư viện tệp .NET Configuration
                var configFile = ConfigurationManager.OpenExeConfiguration(
                    ConfigurationUserLevel.None
                );
                var settings = configFile.ConnectionStrings.ConnectionStrings["QuanLyNhanVien"];

                if (settings == null)
                {
                    settings = new ConnectionStringSettings(
                        "QuanLyNhanVien",
                        _finalConnectionString,
                        "System.Data.SqlClient"
                    );
                    configFile.ConnectionStrings.ConnectionStrings.Add(settings);
                }
                else
                {
                    settings.ConnectionString = _finalConnectionString;
                }

                configFile.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("connectionStrings");

                AppLogger.Info(
                    "FormConnectionWizard",
                    "Connection string đã được cập nhật thành công."
                );

                return true;
            }
            catch (Exception ex)
            {
                AppLogger.Error(
                    "FormConnectionWizard.SaveConfiguration",
                    "Không thể lưu cấu hình.",
                    ex
                );

                MessageBox.Show(
                    "Không thể tự động lưu cấu hình.\n\n"
                        + "Vui lòng cập nhật file App.config thủ công:\n"
                        + "connectionString=\""
                        + _finalConnectionString
                        + "\"\n\n"
                        + "Lỗi: "
                        + ex.Message,
                    "Cảnh Báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return false;
            }
        }

        // ── Triệu Tập Kích Hoạt Sự Kiện ──
        private void BtnNext_Click(object sender, EventArgs e)
        {
            switch (_currentStep)
            {
                case 1:
                    if (string.IsNullOrWhiteSpace(txtServer.Text))
                    {
                        MessageBox.Show(
                            "Vui lòng nhập tên máy chủ hoặc IP!",
                            "Thiếu thông tin",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning
                        );
                        txtServer.Focus();
                        return;
                    }
                    ShowStep(2);
                    break;

                case 2:
                    if (string.IsNullOrWhiteSpace(txtUsername.Text))
                    {
                        MessageBox.Show(
                            "Vui lòng nhập tên đăng nhập SQL Server!",
                            "Thiếu thông tin",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning
                        );
                        txtUsername.Focus();
                        return;
                    }
                    ShowStep(3);
                    break;

                case 3:
                    if (_connectionSucceeded)
                    {
                        ShowStep(4);
                    }
                    else
                    {
                        // Trả thử lại bảng chẩn đoán lỗi
                        ShowStep(3);
                    }
                    break;

                case 4:
                    if (_connectionSucceeded)
                    {
                        if (SaveConfiguration())
                        {
                            ConfigurationSaved = true;
                            MessageBox.Show(
                                "Cấu hình kết nối đã được lưu thành công!",
                                "Thành Công",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information
                            );
                            this.DialogResult = DialogResult.OK;
                            this.Close();
                        }
                    }
                    else
                    {
                        ShowStep(1);
                    }
                    break;
            }
        }

        private void BtnBack_Click(object sender, EventArgs e)
        {
            if (_currentStep > 1)
                ShowStep(_currentStep - 1);
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show(
                "Bạn có chắc muốn thoát wizard?\n"
                    + "Ứng dụng sẽ đóng nếu chưa có cấu hình kết nối.",
                "Xác Nhận",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result == DialogResult.Yes)
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
        }
    }
}
