using System;
using System.Drawing;
using System.Windows.Forms;
using FontAwesome.Sharp;

namespace QuanLyNhanVien.Forms
{
    public partial class FormMain : Form
    {
        // === TRẠNG THÁI ===
        private string _currentUser;
        private Timer _clockTimer;
        private bool _isCollapsed = false;
        private Form _activeForm = null;
        private QuanLyNhanVien.Controls.RoundedButton _currentButton = null;

        private class MenuButtonData
        {
            public string Icon { get; set; }
            public string FullText { get; set; }
            public Color ThemeColor { get; set; }
        }

        public FormMain(string currentUser)
        {
            _currentUser = currentUser;
            InitializeComponent();
            ApplyTheme();
            WireEvents();
            SetAppIcon();
            StartClock();

            // Mặc định tải trang tổng quan (Dashboard)
            OpenChildForm(new FormDashboard(), btnDashboard, "Dashboard");
        }

        private void ApplyTheme()
        {
            this.BackColor = AppColors.Base;

            // Biểu trưng (Logo)
            try
            {
                string logoPath = System.IO.Path.Combine(
                    Application.StartupPath,
                    "Assets",
                    "logo.png"
                );
                if (System.IO.File.Exists(logoPath))
                    pbLogo.Image = Image.FromFile(logoPath);
            }
            catch { }

            // Tiêu đề thanh điều hướng bên (Sidebar labels)
            lblAppName.Font = AppFonts.Create(14, FontStyle.Bold);
            lblAppName.ForeColor = AppColors.Green;
            lblWelcome.Font = AppFonts.Tiny;
            lblWelcome.ForeColor = AppColors.Lavender;
            lblWelcome.Text = "👤 " + _currentUser;
            lblMenu.Font = AppFonts.Create(8, FontStyle.Bold);
            lblMenu.ForeColor = AppColors.Overlay;
            btnToggle.Font = AppFonts.Create(20, FontStyle.Bold);
            btnToggle.ForeColor = AppColors.Text;

            // Phần đầu trang (Header)
            lblPageTitle.Font = AppFonts.Create(14, FontStyle.Bold);
            lblPageTitle.ForeColor = AppColors.Text;
            lblDateTime.Font = AppFonts.Tiny;
            lblDateTime.ForeColor = AppColors.SubText;
            pnlHeader.BackColor = AppColors.Mantle;
            pnlContent.BackColor = AppColors.Base;
            _pnlRightSide.BackColor = AppColors.Base;

            // Ký hiệu (Icon) cho các nút điều hướng
            ApplyNavIcon(btnDashboard, AppIcons.Home);
            ApplyNavIcon(btnNhanVien, AppIcons.Users);
            ApplyNavIcon(btnBoPhan, AppIcons.Building);
            ApplyNavIcon(btnBangLuong, AppIcons.Money);
            ApplyNavIcon(btnThongKe, AppIcons.Chart);
            ApplyNavIcon(btnDangXuat, AppIcons.Logout);

            // Phông chữ nút điều hướng + Dữ liệu gán (Tag data)
            SetNavButtonTag(btnDashboard, "   Trang Chủ");
            SetNavButtonTag(btnNhanVien, "   Nhân Viên");
            SetNavButtonTag(btnBoPhan, "   Bộ Phận");
            SetNavButtonTag(btnBangLuong, "   Tính Lương");
            SetNavButtonTag(btnThongKe, "   Thống Kê");
            SetNavButtonTag(btnDangXuat, "   Đăng Xuất");
        }

        private void ApplyNavIcon(QuanLyNhanVien.Controls.RoundedButton btn, Image icon)
        {
            btn.Font = AppFonts.Small;
            btn.ForeColor = AppColors.Text;
            if (icon != null)
                btn.Image = icon;
        }

        private void SetNavButtonTag(QuanLyNhanVien.Controls.RoundedButton btn, string text)
        {
            btn.Tag = new MenuButtonData
            {
                Icon = "",
                FullText = text,
                ThemeColor = btn.AccentColor,
            };
            btn.AccentColor = Color.Empty; // Ẩn dải màu theo mặc định
            btn.IdleColor = Color.Transparent; // Ẩn hình nền theo mặc định
        }

        private void WireEvents()
        {
            btnToggle.Click += (s, e) => ToggleSidebar();
            btnDashboard.Click += (s, e) =>
                OpenChildForm(new FormDashboard(), btnDashboard, "Dashboard");
            btnNhanVien.Click += (s, e) =>
                OpenChildForm(new FormNhanVien(), btnNhanVien, "Quản Lý Nhân Viên");
            btnBoPhan.Click += (s, e) =>
                OpenChildForm(new FormBoPhan(), btnBoPhan, "Phòng Ban - Chức Vụ");
            btnBangLuong.Click += (s, e) =>
                OpenChildForm(new FormBangLuong(), btnBangLuong, "Bảng Lương Tháng");
            btnThongKe.Click += (s, e) =>
                OpenChildForm(new FormThongKe(), btnThongKe, "Báo Cáo - Thống Kê");
            btnDangXuat.Click += BtnDangXuat_Click;
        }

        private void SetAppIcon()
        {
            try
            {
                string logoPath = System.IO.Path.Combine(
                    Application.StartupPath,
                    "Assets",
                    "logo.png"
                );
                if (System.IO.File.Exists(logoPath))
                {
                    using (Bitmap bmp = new Bitmap(logoPath))
                    {
                        var hIcon = bmp.GetHicon();
                        this.Icon = System.Drawing.Icon.FromHandle(hIcon);
                    }
                }
            }
            catch
            { /* Bỏ qua các lỗi tải biểu tượng icon */
            }
        }

        #region Logic

        private void ToggleSidebar()
        {
            _isCollapsed = !_isCollapsed;

            if (_isCollapsed)
            {
                tblMainLayout.ColumnStyles[0].Width = 70;
                pnlSidebar.Width = 70;
                btnToggle.Location = new Point(15, 10);

                // Ẩn nhãn chữ
                lblAppName.Visible = false;
                lblWelcome.Visible = false;
                lblMenu.Visible = false;

                // Điều chỉnh hình ảnh logo cho chế độ thu gọn
                pbLogo.Width = 70;
                pbLogo.Height = 40;
                pbLogo.Location = new Point(0, 40);

                UpdateButtonState(btnDashboard, true, IconChar.ChartPie);
                UpdateButtonState(btnNhanVien, true, IconChar.Users);
                UpdateButtonState(btnBoPhan, true, IconChar.Building);
                UpdateButtonState(btnBangLuong, true, IconChar.CreditCard);
                UpdateButtonState(btnThongKe, true, IconChar.ChartColumn);
                UpdateButtonState(btnDangXuat, true, IconChar.RightFromBracket);
            }
            else
            {
                tblMainLayout.ColumnStyles[0].Width = 260;
                pnlSidebar.Width = 260;
                btnToggle.Location = new Point(210, 10);

                lblAppName.Visible = true;
                lblWelcome.Visible = true;
                lblMenu.Visible = true;

                // Khôi phục hình ảnh logo
                pbLogo.Width = 260;
                pbLogo.Height = 60;
                pbLogo.Location = new Point(0, 30);

                UpdateButtonState(btnDashboard, false, IconChar.ChartPie);
                UpdateButtonState(btnNhanVien, false, IconChar.Users);
                UpdateButtonState(btnBoPhan, false, IconChar.Building);
                UpdateButtonState(btnBangLuong, false, IconChar.CreditCard);
                UpdateButtonState(btnThongKe, false, IconChar.ChartColumn);
                UpdateButtonState(btnDangXuat, false, IconChar.RightFromBracket);
            }
        }

        private void UpdateButtonState(
            QuanLyNhanVien.Controls.RoundedButton btn,
            bool collapsed,
            IconChar icon
        )
        {
            var data = (MenuButtonData)btn.Tag;
            if (collapsed)
            {
                btn.Text = ""; // Ẩn chữ (No text)
                btn.Image = AppIcons.Get(icon, AppColors.Text, 22);
                btn.Size = new Size(50, 50);
                btn.Location = new Point(10, btn.Location.Y);
                btn.TextAlign = ContentAlignment.MiddleCenter;
                btn.ImageAlign = ContentAlignment.MiddleCenter;
                btn.Padding = new Padding(0);
            }
            else
            {
                btn.Text = data.FullText;
                btn.Image = AppIcons.Get(icon, AppColors.Text, 20);
                btn.Size = new Size(224, 46);
                btn.Location = new Point(18, btn.Location.Y);
                btn.TextAlign = ContentAlignment.MiddleLeft;
                btn.ImageAlign = ContentAlignment.MiddleLeft;
                btn.Padding = new Padding(20, 0, 10, 0);
            }
        }

        public void OpenChildForm(
            Form childForm,
            QuanLyNhanVien.Controls.RoundedButton senderBtn,
            string title
        )
        {
            if (_activeForm != null)
            {
                _activeForm.Close();
                _activeForm.Dispose();
            }

            _activeForm = childForm;

            // Định dạng nút trước đó (Đưa về trạng thái không hoạt động)
            if (_currentButton != null)
            {
                _currentButton.AccentColor = Color.Empty;
                _currentButton.IdleColor = Color.Transparent;
                _currentButton.ForeColor = AppColors.Text;
            }

            _currentButton = senderBtn;

            // Làm nổi bật nút ấn mới (Đặt trạng thái đang xem)
            if (_currentButton != null)
            {
                var tagData = _currentButton.Tag as MenuButtonData;
                Color activeColor =
                    (tagData != null && tagData.ThemeColor != Color.Empty)
                        ? tagData.ThemeColor
                        : AppColors.Blue;

                _currentButton.AccentColor = activeColor;
                _currentButton.IdleColor = Color.FromArgb(40, activeColor);
                _currentButton.ForeColor = activeColor;
            }

            // Thiết lập cửa sổ con mới
            childForm.TopLevel = false;
            childForm.FormBorderStyle = FormBorderStyle.None;
            childForm.Dock = DockStyle.Fill;

            pnlContent.Controls.Add(childForm);
            pnlContent.Tag = childForm;
            childForm.BringToFront();
            childForm.Show();

            lblPageTitle.Text = title;
        }

        private void BtnDangXuat_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show(
                "Bạn đàng muốn đăng xuất khỏi hệ thống?",
                "Xác nhận",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result == DialogResult.Yes)
            {
                _clockTimer?.Stop();
                this.Hide();
                var login = new FormLogin();
                login.FormClosed += (s, args) => Application.Exit();
                login.Show();
            }
        }

        private void StartClock()
        {
            _clockTimer = new Timer { Interval = 1000 };
            _clockTimer.Tick += (s, e) =>
            {
                // Ép ngôn ngữ sang chuẩn tiếng Việt (ngôn ngữ lịch)
                var viCulture = new System.Globalization.CultureInfo("vi-VN");
                lblDateTime.Text = DateTime.Now.ToString("dddd, dd/MM/yyyy HH:mm", viCulture);

                // Viết hoa chữ cái đầu tiên
                if (lblDateTime.Text.Length > 0)
                    lblDateTime.Text =
                        char.ToUpper(lblDateTime.Text[0]) + lblDateTime.Text.Substring(1);
            };
            _clockTimer.Start();
        }

        #endregion

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _clockTimer?.Stop();
                _clockTimer?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
