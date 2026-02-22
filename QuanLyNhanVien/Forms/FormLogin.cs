using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using QuanLyNhanVien.Infrastructure;
using QuanLyNhanVien.Services;

namespace QuanLyNhanVien.Forms
{
    public partial class FormLogin : Form
    {
        private readonly TaiKhoanService _service = new TaiKhoanService();

        public FormLogin()
        {
            InitializeComponent();
            ApplyTheme();
            WireEvents();
            SetAppIcon();

            // Tải thông tin đăng nhập đã lưu
            var settings = LoginSettings.Load();
            if (settings != null)
            {
                txtUser.Text = settings.Username;
                txtPass.Text = SecurityHelper.Decrypt(settings.EncryptedPassword);
                chkRemember.Checked = settings.RememberMe;
            }
        }

        private void ApplyTheme()
        {
            this.BackColor = AppColors.Crust;
            pnlCard.BackColor = AppColors.Base;

            // Biểu trưng (Logo)
            try
            {
                string logoPath = System.IO.Path.Combine(
                    Application.StartupPath,
                    "Assets",
                    "logo.png"
                );
                if (System.IO.File.Exists(logoPath))
                {
                    var bmp = new Bitmap(logoPath);
                    pbLogo.Image = bmp;
                    this.Icon = System.Drawing.Icon.FromHandle(bmp.GetHicon());
                }
            }
            catch { }

            // Tiêu đề
            lblTitle.Font = AppFonts.Create(16, FontStyle.Bold);
            lblTitle.ForeColor = AppColors.Green;

            lblSubtitle.Font = AppFonts.Tiny;
            lblSubtitle.ForeColor = AppColors.Overlay;

            // Điền chữ nhãn
            lblUser.Font = AppFonts.Tiny;
            lblUser.ForeColor = AppColors.SubText;
            lblPass.Font = AppFonts.Tiny;
            lblPass.ForeColor = AppColors.SubText;

            // Ô nhập liệu
            txtUser.Font = AppFonts.Body;
            txtUser.BackColor = AppColors.InputBg;
            txtPass.Font = AppFonts.Body;
            txtPass.BackColor = AppColors.InputBg;

            // Hộp kiểm (Checkbox)
            chkRemember.Font = AppFonts.Tiny;
            chkRemember.ForeColor = AppColors.SubText;

            // Các nút ấn
            btnLogin.Font = AppFonts.BodyBold;
            btnLogin.ForeColor = AppColors.Crust;
            btnLogin.IdleColor = AppColors.Blue;
            btnLogin.HoverColor = AppColors.Lighten(AppColors.Blue);
            btnLogin.PressColor = AppColors.Darken(AppColors.Blue);

            btnExit.Font = AppFonts.BodyBold;
            btnExit.ForeColor = AppColors.Crust;
            btnExit.IdleColor = AppColors.Red;
            btnExit.HoverColor = AppColors.Lighten(AppColors.Red);
            btnExit.PressColor = AppColors.Darken(AppColors.Red);

            // Trạng thái
            lblStatus.Font = AppFonts.Tiny;
            lblStatus.ForeColor = AppColors.Red;
        }

        private void WireEvents()
        {
            btnLogin.Click += BtnLogin_Click;
            btnExit.Click += (s, e) => Application.Exit();
            pnlCard.Paint += PnlCard_Paint;
            txtUser.KeyDown += InputKeyDown;
            txtPass.KeyDown += InputKeyDown;
        }

        private void SetAppIcon()
        {
            try
            {
                string iconPath = System.IO.Path.Combine(
                    Application.StartupPath,
                    "Assets",
                    "app.ico"
                );
                if (System.IO.File.Exists(iconPath))
                {
                    this.Icon = new Icon(iconPath);
                }
            }
            catch
            { /* Bỏ qua các lỗi tải biểu tượng icon */
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
        /// Xử lý đăng nhập — ủy quyền kiểm tra bảo mật sang TaiKhoanService.
        /// Form chỉ đảm nhiệm phản hồi lại giao diện mức UI cho người sử dụng.
        /// </summary>
        private void BtnLogin_Click(object sender, EventArgs e)
        {
            lblStatus.Text = "";

            try
            {
                var result = _service.DangNhap(txtUser.Text, txtPass.Text);

                if (result.Success)
                {
                    // Lưu cấu hình đăng nhập
                    LoginSettings.Save(txtUser.Text, txtPass.Text, chkRemember.Checked);

                    // Theo dõi tên người dùng đã đăng nhập cho tất cả các bản ghi nhật ký tiếp theo
                    AppLogger.SetCurrentUser(result.Data.TenDangNhap);
                    AppLogger.Info("FormLogin", "Đăng nhập thành công: " + result.Data.TenDangNhap);

                    this.Hide();
                    var main = new FormMain(result.Data.TenDangNhap);
                    main.FormClosed += (s, args) => Application.Exit();
                    main.Show();
                }
                else
                {
                    AppLogger.Warning(
                        "FormLogin",
                        "Đăng nhập thất bại cho: " + txtUser.Text + " — " + result.Message
                    );

                    lblStatus.Text = result.Message;
                    txtPass.Clear();
                    txtPass.Focus();
                }
            }
            catch (Exception ex)
            {
                AppLogger.Error("FormLogin.BtnLogin_Click", "Lỗi kết nối CSDL khi đăng nhập.", ex);

                MessageBox.Show(
                    "Lỗi kết nối CSDL:\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }
    }
}
