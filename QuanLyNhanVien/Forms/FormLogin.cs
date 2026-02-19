using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using QuanLyNhanVien.Controls;
using QuanLyNhanVien.Infrastructure;
using QuanLyNhanVien.Services;

namespace QuanLyNhanVien.Forms
{
    public class FormLogin : Form
    {
        private void SetAppIcon()
        {
            try
            {
                string iconPath = System.IO.Path.Combine(Application.StartupPath, "Assets", "app.ico");
                if (System.IO.File.Exists(iconPath))
                {
                    this.Icon = new Icon(iconPath);
                }
            }
            catch { /* Ignore icon loading errors */ }
        }

        private Panel pnlCard;
        private PictureBox pbLogo;
        private Label lblTitle;
        private Label lblSubtitle;
        private Label lblUser;
        private Label lblPass;
        private TextBox txtUser;
        private TextBox txtPass;
        private RoundedButton btnLogin;
        private RoundedButton btnExit;
        private Label lblStatus;
        private CheckBox chkRemember;

        private readonly TaiKhoanService _service = new TaiKhoanService();

        public FormLogin()
        {
            InitializeComponent();
            SetAppIcon();
            txtUser.KeyDown += InputKeyDown;
            txtPass.KeyDown += InputKeyDown;

            // Load saved credentials
            var settings = LoginSettings.Load();
            if (settings != null)
            {
                txtUser.Text = settings.Username;
                txtPass.Text = SecurityHelper.Decrypt(settings.EncryptedPassword);
                chkRemember.Checked = settings.RememberMe;
            }
        }

        private void InputKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                BtnLogin_Click(sender, e);
            }
            else if (e.KeyCode == Keys.Escape)
            {
                Application.Exit();
            }
        }

        private void InitializeComponent()
        {
            this.Text = "Đăng Nhập — Quản Lý Nhân Viên Quán Ăn";
            this.Size = new Size(500, 440);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = AppColors.Crust;

            // Central card panel
            pnlCard = new Panel
            {
                Size = new Size(420, 370),
                Location = new Point(40, 30),
                BackColor = AppColors.Base
            };
            pnlCard.Paint += PnlCard_Paint;

            // Logo
            pbLogo = new PictureBox
            {
                Image = Image.FromFile("Assets/logo.png"),
                SizeMode = PictureBoxSizeMode.Zoom,
                Location = new Point(0, 15),
                Size = new Size(420, 50),
                BackColor = Color.Transparent
            };

            // Title
            lblTitle = new Label
            {
                Text = "QUẢN LÝ NHÂN VIÊN",
                Font = AppFonts.Create(16, FontStyle.Bold),
                ForeColor = AppColors.Green,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(0, 65),
                Size = new Size(420, 28),
                BackColor = Color.Transparent
            };

            // Subtitle
            lblSubtitle = new Label
            {
                Text = "Đăng nhập để tiếp tục",
                Font = AppFonts.Tiny,
                ForeColor = AppColors.Overlay,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(0, 92),
                Size = new Size(420, 20),
                BackColor = Color.Transparent
            };

            // Username
            lblUser = new Label
            {
                Text = "Tên đăng nhập",
                Font = AppFonts.Tiny,
                ForeColor = AppColors.SubText,
                Location = new Point(40, 130),
                AutoSize = true,
                BackColor = Color.Transparent
            };

            txtUser = new TextBox
            {
                Font = AppFonts.Body,
                Location = new Point(40, 150),
                Size = new Size(340, 30),
                BackColor = AppColors.InputBg,
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            // Password
            lblPass = new Label
            {
                Text = "Mật khẩu",
                Font = AppFonts.Tiny,
                ForeColor = AppColors.SubText,
                Location = new Point(40, 192),
                AutoSize = true,
                BackColor = Color.Transparent
            };

            txtPass = new TextBox
            {
                Font = AppFonts.Body,
                Location = new Point(40, 212),
                Size = new Size(340, 30),
                PasswordChar = '●',
                BackColor = AppColors.InputBg,
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            // Remember checkbox
            chkRemember = new CheckBox
            {
                Text = "Ghi nhớ đăng nhập",
                Font = AppFonts.Tiny,
                ForeColor = AppColors.SubText,
                Location = new Point(40, 250),
                AutoSize = true,
                BackColor = Color.Transparent
            };

            // Actions Panel to group buttons
            var pnlActions = new FlowLayoutPanel
            {
                Location = new Point(40, 285),
                Size = new Size(340, 50),
                BackColor = Color.Transparent,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false
            };

            // Login button
            btnLogin = new RoundedButton
            {
                Text = "ĐĂNG NHẬP",
                Font = AppFonts.BodyBold,
                Size = new Size(165, 42),
                Margin = new Padding(0, 0, 10, 0),
                IdleColor = AppColors.Blue,
                HoverColor = AppColors.Lighten(AppColors.Blue),
                PressColor = AppColors.Darken(AppColors.Blue),
                ForeColor = AppColors.Crust,
                CornerRadius = 10,
                TextAlign = ContentAlignment.MiddleCenter
            };
            btnLogin.Click += BtnLogin_Click;

            // Exit button
            btnExit = new RoundedButton
            {
                Text = "THOÁT",
                Font = AppFonts.BodyBold,
                Size = new Size(165, 42),
                Margin = new Padding(0),
                IdleColor = AppColors.Red,
                HoverColor = AppColors.Lighten(AppColors.Red),
                PressColor = AppColors.Darken(AppColors.Red),
                ForeColor = AppColors.Crust,
                CornerRadius = 10,
                TextAlign = ContentAlignment.MiddleCenter
            };
            btnExit.Click += (s, e) => Application.Exit();

            pnlActions.Controls.Add(btnLogin);
            pnlActions.Controls.Add(btnExit);

            // Status label
            lblStatus = new Label
            {
                Text = "",
                Font = AppFonts.Tiny,
                ForeColor = AppColors.Red,
                Location = new Point(40, 340),
                Size = new Size(340, 20),
                BackColor = Color.Transparent
            };

            pnlCard.Controls.AddRange(new Control[]
            {
                pbLogo, lblTitle, lblSubtitle,
                lblUser, txtUser, lblPass, txtPass,
                chkRemember,
                pnlActions, lblStatus
            });
            this.Controls.Add(pnlCard);
        }

        private void PnlCard_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            var rect = new Rectangle(0, 0, pnlCard.Width - 1, pnlCard.Height - 1);
            using (var pen = new Pen(Color.FromArgb(30, 166, 227, 161), 1f))
            {
                int r = 14;
                int d = r * 2;
                var path = new GraphicsPath();
                path.AddArc(rect.X, rect.Y, d, d, 180, 90);
                path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
                path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
                path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
                path.CloseFigure();
                g.DrawPath(pen, path);
                path.Dispose();
            }
        }

        /// <summary>
        /// Login handler — delegates validation and auth to TaiKhoanService.
        /// The Form only handles UI feedback.
        /// </summary>
        private void BtnLogin_Click(object sender, EventArgs e)
        {
            lblStatus.Text = "";

            try
            {
                var result = _service.DangNhap(txtUser.Text, txtPass.Text);

                if (result.Success)
                {
                    // Save login settings
                    LoginSettings.Save(txtUser.Text, txtPass.Text, chkRemember.Checked);

                    // Track the logged-in user for all subsequent log entries
                    AppLogger.SetCurrentUser(result.Data.TenDangNhap);
                    AppLogger.Info("FormLogin",
                        "Đăng nhập thành công: " + result.Data.TenDangNhap);

                    this.Hide();
                    var main = new FormMain(result.Data.TenDangNhap);
                    main.FormClosed += (s, args) => Application.Exit();
                    main.Show();
                }
                else
                {
                    AppLogger.Warning("FormLogin",
                        "Đăng nhập thất bại cho: " + txtUser.Text
                        + " — " + result.Message);

                    lblStatus.Text = "❌ " + result.Message;
                    txtPass.Clear();
                    txtPass.Focus();
                }
            }
            catch (Exception ex)
            {
                AppLogger.Error("FormLogin.BtnLogin_Click",
                    "Lỗi kết nối CSDL khi đăng nhập.", ex);

                MessageBox.Show("Lỗi kết nối CSDL:\n" + ex.Message,
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
