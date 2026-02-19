using System;
using System.Configuration;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading;
using System.Windows.Forms;
using QuanLyNhanVien.Controls;
using QuanLyNhanVien.Infrastructure;

namespace QuanLyNhanVien.Forms
{
    /// <summary>
    /// Database Connection Wizard — guides clients through configuring
    /// their SQL Server connection on first run or when the connection fails.
    /// 
    /// Uses a systematic 4-step diagnostic approach:
    ///   Step 1: Server address + port
    ///   Step 2: Credentials
    ///   Step 3: Automated diagnostic (TCP → Auth → DB → Schema)
    ///   Step 4: Save configuration
    /// 
    /// Designed for deployment at client sites where the IT staff needs to
    /// configure the connection to their SQL Server instance.
    /// </summary>
    public class FormConnectionWizard : Form
    {
        // ── Controls ──
        private Panel pnlCard;
        private Label lblLogo;
        private Label lblTitle;
        private Label lblStepTitle;
        private Label lblStepDesc;

        // Step 1: Server
        private Panel pnlStep1;
        private Label lblServer;
        private TextBox txtServer;
        private Label lblPort;
        private TextBox txtPort;

        // Step 2: Credentials
        private Panel pnlStep2;
        private Label lblUsername;
        private TextBox txtUsername;
        private Label lblPassword;
        private TextBox txtPassword;
        private Label lblDatabase;
        private TextBox txtDatabase;

        // Step 3: Diagnostics
        private Panel pnlStep3;
        private RichTextBox rtbDiagnostic;
        private ProgressBar pbDiagnostic;

        // Step 4: Result
        private Panel pnlStep4;
        private Label lblResult;

        // Navigation
        private RoundedButton btnBack;
        private RoundedButton btnNext;
        private RoundedButton btnCancel;
        private Label lblStepIndicator;

        // ── State ──
        private int _currentStep = 1;
        private const int TOTAL_STEPS = 4;
        private bool _connectionSucceeded;
        private string _finalConnectionString;

        /// <summary>
        /// True if the wizard completed successfully and updated App.config.
        /// </summary>
        public bool ConfigurationSaved { get; private set; }

        public FormConnectionWizard()
        {
            InitializeComponent();
            ShowStep(1);
            PreFillFromConfig();
        }

        // ── Pre-fill from existing App.config ──
        private void PreFillFromConfig()
        {
            try
            {
                var cs = ConfigurationManager.ConnectionStrings["QuanLyNhanVien"];
                if (cs != null && !string.IsNullOrEmpty(cs.ConnectionString))
                {
                    var builder = new System.Data.SqlClient.SqlConnectionStringBuilder(cs.ConnectionString);
                    string dataSource = builder.DataSource ?? "";

                    // Parse server,port from DataSource
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
                // Failed to parse existing config — ignore, user will enter manually
            }
        }

        // ── Step Navigation ──
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
                        ? "✅ Kết Nối Thành Công!"
                        : "❌ Kết Nối Thất Bại";
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

        // ── Diagnostic Execution ──
        private void RunDiagnosticsAsync()
        {
            rtbDiagnostic.Clear();
            pbDiagnostic.Value = 0;
            _connectionSucceeded = false;

            string server = txtServer.Text.Trim();
            int port;
            if (!int.TryParse(txtPort.Text.Trim(), out port)) port = 1433;
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;
            string database = txtDatabase.Text.Trim();

            // Run in background thread to keep UI responsive
            var bgThread = new Thread(() =>
            {
                try
                {
                    // Step by step with UI updates
                    AppendDiagnostic("🔍 Bắt đầu kiểm tra kết nối...\n", Color.White);
                    UpdateProgress(5);

                    // 1. TCP
                    AppendDiagnostic("\n━━━ 1. Kiểm tra kết nối mạng (TCP) ━━━\n", AppColors.Blue);
                    var tcpResult = ConnectionDiagnostics.TestTcpConnectivity(server, port);
                    ShowDiagnosticResult(tcpResult);
                    UpdateProgress(25);
                    if (!tcpResult.Success) { FinishDiagnostic(false); return; }

                    // 2. Auth
                    AppendDiagnostic("\n━━━ 2. Kiểm tra đăng nhập ━━━\n", AppColors.Blue);
                    var authResult = ConnectionDiagnostics.TestAuthentication(server, port, username, password);
                    ShowDiagnosticResult(authResult);
                    UpdateProgress(50);
                    if (!authResult.Success) { FinishDiagnostic(false); return; }

                    // 3. Database
                    AppendDiagnostic("\n━━━ 3. Kiểm tra cơ sở dữ liệu ━━━\n", AppColors.Blue);
                    var dbResult = ConnectionDiagnostics.TestDatabaseExists(
                        server, port, username, password, database);
                    ShowDiagnosticResult(dbResult);
                    UpdateProgress(75);
                    if (!dbResult.Success) { FinishDiagnostic(false); return; }

                    // 4. Schema
                    AppendDiagnostic("\n━━━ 4. Kiểm tra cấu trúc bảng ━━━\n", AppColors.Blue);
                    var schemaResult = ConnectionDiagnostics.TestSchemaReady(
                        server, port, username, password, database);
                    ShowDiagnosticResult(schemaResult);
                    UpdateProgress(100);

                    if (schemaResult.Success)
                    {
                        _finalConnectionString = ConnectionDiagnostics.BuildConnectionString(
                            server, port, username, password, database);
                        AppendDiagnostic("\n\n✅ Tất cả kiểm tra đều thành công!\n", AppColors.Green);
                        FinishDiagnostic(true);
                    }
                    else
                    {
                        // Schema not ready — still allow saving (user can init DB later)
                        _finalConnectionString = ConnectionDiagnostics.BuildConnectionString(
                            server, port, username, password, database);
                        AppendDiagnostic(
                            "\n\n⚠️ Kết nối thành công nhưng cơ sở dữ liệu chưa sẵn sàng.\n"
                            + "Bạn vẫn có thể lưu cấu hình và khởi tạo database sau.\n",
                            AppColors.Yellow);
                        FinishDiagnostic(true);
                    }
                }
                catch (Exception ex)
                {
                    AppendDiagnostic("\n\n❌ Lỗi không xác định: " + ex.Message + "\n", AppColors.Red);
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
                AppendDiagnostic("  ✅ " + result.Message + "\n", AppColors.Green);
            }
            else
            {
                AppendDiagnostic("  ❌ " + result.Message + "\n", AppColors.Red);
                if (!string.IsNullOrEmpty(result.Suggestion))
                {
                    AppendDiagnostic("\n  💡 Gợi ý khắc phục:\n", AppColors.Yellow);
                    AppendDiagnostic("  " + result.Suggestion.Replace("\n", "\n  ") + "\n",
                        AppColors.SubText);
                }
            }
        }

        // ── Thread-safe UI helpers ──
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
            // Show navigation buttons again
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

        // ── Save Configuration ──
        private bool SaveConfiguration()
        {
            try
            {
                // Update the App.config file using .NET Configuration API
                var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                var settings = configFile.ConnectionStrings.ConnectionStrings["QuanLyNhanVien"];

                if (settings == null)
                {
                    settings = new ConnectionStringSettings(
                        "QuanLyNhanVien",
                        _finalConnectionString,
                        "System.Data.SqlClient");
                    configFile.ConnectionStrings.ConnectionStrings.Add(settings);
                }
                else
                {
                    settings.ConnectionString = _finalConnectionString;
                }

                configFile.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("connectionStrings");

                AppLogger.Info("FormConnectionWizard",
                    "Connection string đã được cập nhật thành công.");

                return true;
            }
            catch (Exception ex)
            {
                AppLogger.Error("FormConnectionWizard.SaveConfiguration",
                    "Không thể lưu cấu hình.", ex);

                MessageBox.Show(
                    "Không thể tự động lưu cấu hình.\n\n"
                    + "Vui lòng cập nhật file App.config thủ công:\n"
                    + "connectionString=\"" + _finalConnectionString + "\"\n\n"
                    + "Lỗi: " + ex.Message,
                    "Cảnh Báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
        }

        // ── Event Handlers ──
        private void BtnNext_Click(object sender, EventArgs e)
        {
            switch (_currentStep)
            {
                case 1:
                    if (string.IsNullOrWhiteSpace(txtServer.Text))
                    {
                        MessageBox.Show("Vui lòng nhập tên máy chủ hoặc IP!",
                            "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtServer.Focus();
                        return;
                    }
                    ShowStep(2);
                    break;

                case 2:
                    if (string.IsNullOrWhiteSpace(txtUsername.Text))
                    {
                        MessageBox.Show("Vui lòng nhập tên đăng nhập SQL Server!",
                            "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                        // Retry diagnostics
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
                                "Thành Công", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
        }

        // ── UI Setup ──
        private void InitializeComponent()
        {
            this.Text = "Trình Hướng Dẫn Kết Nối Database — Quản Lý Nhân Viên";
            this.Size = new Size(580, 560);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = AppColors.Crust;

            // ── Card panel ──
            pnlCard = new Panel
            {
                Size = new Size(540, 490),
                Location = new Point(20, 15),
                BackColor = AppColors.Base
            };
            pnlCard.Paint += (s, e) =>
            {
                var g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                var rect = new Rectangle(0, 0, pnlCard.Width - 1, pnlCard.Height - 1);
                using (var pen = new Pen(Color.FromArgb(30, 166, 227, 161), 1f))
                {
                    int r = 14, d = r * 2;
                    var path = new GraphicsPath();
                    path.AddArc(rect.X, rect.Y, d, d, 180, 90);
                    path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
                    path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
                    path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
                    path.CloseFigure();
                    g.DrawPath(pen, path);
                    path.Dispose();
                }
            };

            // ── Header ──
            lblLogo = new Label
            {
                Text = "🔌",
                Font = AppFonts.Create(32),
                ForeColor = AppColors.Blue,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(0, 10),
                Size = new Size(540, 42),
                BackColor = Color.Transparent
            };

            lblTitle = new Label
            {
                Text = "THIẾT LẬP KẾT NỐI CƠ SỞ DỮ LIỆU",
                Font = AppFonts.Create(13, FontStyle.Bold),
                ForeColor = AppColors.Blue,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(0, 52),
                Size = new Size(540, 24),
                BackColor = Color.Transparent
            };

            lblStepTitle = new Label
            {
                Text = "",
                Font = AppFonts.BodyBold,
                ForeColor = AppColors.Text,
                Location = new Point(25, 85),
                Size = new Size(490, 22),
                BackColor = Color.Transparent
            };

            lblStepDesc = new Label
            {
                Text = "",
                Font = AppFonts.Tiny,
                ForeColor = AppColors.Overlay,
                Location = new Point(25, 107),
                Size = new Size(490, 18),
                BackColor = Color.Transparent
            };

            // ── Step 1: Server ──
            pnlStep1 = new Panel
            {
                Location = new Point(25, 135),
                Size = new Size(490, 250),
                BackColor = Color.Transparent
            };

            lblServer = new Label
            {
                Text = "Tên máy chủ hoặc IP:",
                Font = AppFonts.Small,
                ForeColor = AppColors.SubText,
                Location = new Point(0, 10),
                AutoSize = true,
                BackColor = Color.Transparent
            };
            txtServer = new TextBox
            {
                Font = AppFonts.Body,
                Location = new Point(0, 32),
                Size = new Size(490, 30),
                BackColor = AppColors.InputBg,
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Text = "localhost"
            };

            lblPort = new Label
            {
                Text = "Cổng (Port):",
                Font = AppFonts.Small,
                ForeColor = AppColors.SubText,
                Location = new Point(0, 72),
                AutoSize = true,
                BackColor = Color.Transparent
            };
            txtPort = new TextBox
            {
                Font = AppFonts.Body,
                Location = new Point(0, 94),
                Size = new Size(120, 30),
                BackColor = AppColors.InputBg,
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Text = "1433"
            };

            // Helper tips for step 1
            var lblTip1 = new Label
            {
                Text = "💡 Ví dụ:\n"
                    + "  • localhost — SQL Server trên máy này\n"
                    + "  • 192.168.1.100 — IP của máy chủ\n"
                    + "  • MYSERVER\\SQLEXPRESS — Named instance\n"
                    + "  • Cổng mặc định SQL Server: 1433",
                Font = AppFonts.Tiny,
                ForeColor = AppColors.Overlay,
                Location = new Point(0, 140),
                Size = new Size(490, 90),
                BackColor = Color.Transparent
            };

            pnlStep1.Controls.AddRange(new Control[]
                { lblServer, txtServer, lblPort, txtPort, lblTip1 });

            // ── Step 2: Credentials ──
            pnlStep2 = new Panel
            {
                Location = new Point(25, 135),
                Size = new Size(490, 250),
                BackColor = Color.Transparent,
                Visible = false
            };

            lblUsername = new Label
            {
                Text = "Tên đăng nhập SQL:",
                Font = AppFonts.Small,
                ForeColor = AppColors.SubText,
                Location = new Point(0, 10),
                AutoSize = true,
                BackColor = Color.Transparent
            };
            txtUsername = new TextBox
            {
                Font = AppFonts.Body,
                Location = new Point(0, 32),
                Size = new Size(490, 30),
                BackColor = AppColors.InputBg,
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Text = "sa"
            };

            lblPassword = new Label
            {
                Text = "Mật khẩu:",
                Font = AppFonts.Small,
                ForeColor = AppColors.SubText,
                Location = new Point(0, 72),
                AutoSize = true,
                BackColor = Color.Transparent
            };
            txtPassword = new TextBox
            {
                Font = AppFonts.Body,
                Location = new Point(0, 94),
                Size = new Size(490, 30),
                BackColor = AppColors.InputBg,
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                PasswordChar = '●'
            };

            lblDatabase = new Label
            {
                Text = "Tên database:",
                Font = AppFonts.Small,
                ForeColor = AppColors.SubText,
                Location = new Point(0, 134),
                AutoSize = true,
                BackColor = Color.Transparent
            };
            txtDatabase = new TextBox
            {
                Font = AppFonts.Body,
                Location = new Point(0, 156),
                Size = new Size(300, 30),
                BackColor = AppColors.InputBg,
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Text = "QuanLyNhanVien"
            };

            var lblTip2 = new Label
            {
                Text = "💡 Tài khoản mặc định: sa / [mật khẩu khi cài đặt SQL Server]",
                Font = AppFonts.Tiny,
                ForeColor = AppColors.Overlay,
                Location = new Point(0, 200),
                Size = new Size(490, 30),
                BackColor = Color.Transparent
            };

            pnlStep2.Controls.AddRange(new Control[]
                { lblUsername, txtUsername, lblPassword, txtPassword,
                  lblDatabase, txtDatabase, lblTip2 });

            // ── Step 3: Diagnostics ──
            pnlStep3 = new Panel
            {
                Location = new Point(25, 135),
                Size = new Size(490, 280),
                BackColor = Color.Transparent,
                Visible = false
            };

            pbDiagnostic = new ProgressBar
            {
                Location = new Point(0, 0),
                Size = new Size(490, 20),
                Style = ProgressBarStyle.Continuous
            };

            rtbDiagnostic = new RichTextBox
            {
                Location = new Point(0, 28),
                Size = new Size(490, 245),
                BackColor = AppColors.Mantle,
                ForeColor = AppColors.Text,
                Font = AppFonts.Small,
                ReadOnly = true,
                BorderStyle = BorderStyle.None,
                ScrollBars = RichTextBoxScrollBars.Vertical
            };

            pnlStep3.Controls.AddRange(new Control[] { pbDiagnostic, rtbDiagnostic });

            // ── Step 4: Result ──
            pnlStep4 = new Panel
            {
                Location = new Point(25, 135),
                Size = new Size(490, 250),
                BackColor = Color.Transparent,
                Visible = false
            };

            lblResult = new Label
            {
                Text = "",
                Font = AppFonts.Body,
                ForeColor = AppColors.Text,
                Location = new Point(0, 10),
                Size = new Size(490, 200),
                BackColor = Color.Transparent
            };

            pnlStep4.Controls.Add(lblResult);

            // ── Navigation Buttons ──
            int btnY = 440;

            btnBack = new RoundedButton
            {
                Text = "← QUAY LẠI",
                Font = AppFonts.SmallBold,
                Size = new Size(130, 36),
                Location = new Point(25, btnY),
                IdleColor = AppColors.Surface1,
                HoverColor = AppColors.Surface2,
                PressColor = AppColors.Surface0,
                ForeColor = AppColors.Text,
                CornerRadius = 8,
                TextAlign = ContentAlignment.MiddleCenter,
                Visible = false
            };
            btnBack.Click += BtnBack_Click;

            btnCancel = new RoundedButton
            {
                Text = "THOÁT",
                Font = AppFonts.SmallBold,
                Size = new Size(100, 36),
                Location = new Point(250, btnY),
                IdleColor = AppColors.Red,
                HoverColor = AppColors.Lighten(AppColors.Red),
                PressColor = AppColors.Darken(AppColors.Red),
                ForeColor = AppColors.Crust,
                CornerRadius = 8,
                TextAlign = ContentAlignment.MiddleCenter
            };
            btnCancel.Click += BtnCancel_Click;

            btnNext = new RoundedButton
            {
                Text = "TIẾP TỤC →",
                Font = AppFonts.SmallBold,
                Size = new Size(165, 36),
                Location = new Point(365, btnY),
                IdleColor = AppColors.Blue,
                HoverColor = AppColors.Lighten(AppColors.Blue),
                PressColor = AppColors.Darken(AppColors.Blue),
                ForeColor = AppColors.Crust,
                CornerRadius = 8,
                TextAlign = ContentAlignment.MiddleCenter
            };
            btnNext.Click += BtnNext_Click;

            lblStepIndicator = new Label
            {
                Text = "",
                Font = AppFonts.Tiny,
                ForeColor = AppColors.Overlay,
                TextAlign = ContentAlignment.MiddleLeft,
                Location = new Point(25, btnY + 38),
                Size = new Size(200, 16),
                BackColor = Color.Transparent
            };

            // ── Assembly ──
            pnlCard.Controls.AddRange(new Control[]
            {
                lblLogo, lblTitle, lblStepTitle, lblStepDesc,
                pnlStep1, pnlStep2, pnlStep3, pnlStep4,
                btnBack, btnCancel, btnNext, lblStepIndicator
            });
            this.Controls.Add(pnlCard);
        }
    }
}
