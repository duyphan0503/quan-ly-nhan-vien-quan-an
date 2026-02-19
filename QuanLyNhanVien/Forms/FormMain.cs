using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using QuanLyNhanVien.Controls;
using QuanLyNhanVien.Services;

namespace QuanLyNhanVien.Forms
{
    public class FormMain : Form
    {
        // === STATE ===
        private string _currentUser;
        private Timer _clockTimer;
        private bool _isCollapsed = false;
        private Form _activeForm = null;
        private RoundedButton _currentButton = null;

        // === CONTROLS ===
        private GlassPanel pnlSidebar;
        private Panel pnlContent;
        private Panel pnlHeader;
        private TableLayoutPanel tblMainLayout;
        
        // Header items
        private Label lblPageTitle;
        private Label lblDateTime;
        private RoundedButton btnToggle; // The hamburger button

        // Sidebar items
        private PictureBox pbLogo;
        private Label lblAppName;
        private Label lblWelcome;
        private Label lblMenu;
        
        // Navigation Buttons
        private RoundedButton btnDashboard; // Home/Dashboard
        private RoundedButton btnNhanVien;
        private RoundedButton btnBoPhan;
        private RoundedButton btnBangLuong;
        private RoundedButton btnThongKe;
        private RoundedButton btnDangXuat;

        private class MenuButtonData
        {
            public string Icon { get; set; }
            public string FullText { get; set; }
        }

        public FormMain(string currentUser)
        {
            _currentUser = currentUser;
            InitializeComponent();
            SetAppIcon();
            StartClock();
            
            // Load dashboard by default
            OpenChildForm(new FormDashboard(), btnDashboard, "Dashboard");
        }

        private void InitializeComponent()
        {
            this.Text = "Quản Lý Nhân Viên Quán Ăn";
            this.Size = new Size(1200, 720);
            this.MinimumSize = new Size(950, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = AppColors.Base;
            this.DoubleBuffered = true;

            // 1. Build Base Containers
            BuildSidebar();
            BuildContentArea();

            // 2. Main Layout Table
            tblMainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                BackColor = Color.Transparent
            };
            tblMainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 260)); // Sidebar
            tblMainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));  // Content

            tblMainLayout.Controls.Add(pnlSidebar, 0, 0);
            tblMainLayout.Controls.Add(_pnlRightSide, 1, 0);
            
            this.Controls.Add(tblMainLayout);
        }

        private Panel _pnlRightSide;

        #region Sidebar Construction

        private void BuildSidebar()
        {
            pnlSidebar = new GlassPanel
            {
                Dock = DockStyle.Fill, // Changed from Left to Fill (inside Table cell)
                Width = 260,
                GradientTop = Color.FromArgb(220, 22, 22, 35),
                GradientBottom = Color.FromArgb(240, 17, 17, 27),
                GlassBorderColor = Color.FromArgb(50, 166, 227, 161),
                BorderSide = GlassPanel.GlassBorderSide.None, // No border needed if in table
                Padding = new Padding(0)
            };

            // Toggle Button (Hamburger)
            btnToggle = new RoundedButton
            {
                Text = "≡", 
                Font = AppFonts.Create(20, FontStyle.Bold),
                Size = new Size(40, 40),
                Location = new Point(210, 10), // Top right of sidebar
                IdleColor = Color.Transparent,
                HoverColor = Color.FromArgb(30, 255, 255, 255),
                ForeColor = AppColors.Text,
                TextAlign = ContentAlignment.MiddleCenter,
                Padding = new Padding(0), // Center the icon
                CornerRadius = 10
            };
            btnToggle.Click += (s, e) => ToggleSidebar();

            // Logo
            pbLogo = new PictureBox
            {
                Image = Image.FromFile("Assets/logo.png"),
                SizeMode = PictureBoxSizeMode.Zoom,
                Location = new Point(0, 30),
                Size = new Size(260, 60),
                BackColor = Color.Transparent
            };

            lblAppName = new Label
            {
                Text = "QUÁN ĂN",
                Font = AppFonts.Create(14, FontStyle.Bold),
                ForeColor = AppColors.Green,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(0, 90),
                Size = new Size(260, 30),
                BackColor = Color.Transparent
            };

            lblWelcome = new Label
            {
                Text = "👤 " + _currentUser,
                Font = AppFonts.Tiny,
                ForeColor = AppColors.Lavender,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(0, 120),
                Size = new Size(260, 20),
                BackColor = Color.Transparent
            };

            // Menu Label
            lblMenu = new Label
            {
                Text = "MENU",
                Font = AppFonts.Create(8, FontStyle.Bold),
                ForeColor = AppColors.Overlay,
                Location = new Point(22, 160),
                AutoSize = true,
                BackColor = Color.Transparent
            };
            
            // Buttons
            int startY = 190;
            int gap = 8;
            
            // Note: Add extra spaces before icon for padding visual
            btnDashboard = CreateNavButton("🏠", "   🏠   Trang Chủ", startY, AppColors.Mauve);
            btnDashboard.Click += (s, e) => OpenChildForm(new FormDashboard(), btnDashboard, "Dashboard");
            startY += 50 + gap;

            btnNhanVien = CreateNavButton("👥", "   👥   Nhân Viên", startY, AppColors.Green);
            btnNhanVien.Click += (s, e) => OpenChildForm(new FormNhanVien(), btnNhanVien, "Quản Lý Nhân Viên");
            startY += 50 + gap;

            btnBoPhan = CreateNavButton("🏢", "   🏢   Bộ Phận", startY, AppColors.Blue);
            btnBoPhan.Click += (s, e) => OpenChildForm(new FormBoPhan(), btnBoPhan, "Phòng Ban - Chức Vụ");
            startY += 50 + gap;

            btnBangLuong = CreateNavButton("💰", "   💰   Tính Lương", startY, AppColors.Yellow);
            btnBangLuong.Click += (s, e) => OpenChildForm(new FormBangLuong(), btnBangLuong, "Bảng Lương Tháng");
            startY += 50 + gap;

            btnThongKe = CreateNavButton("📊", "   📊   Thống Kê", startY, AppColors.Lavender);
            btnThongKe.Click += (s, e) => OpenChildForm(new FormThongKe(), btnThongKe, "Báo Cáo - Thống Kê");
            startY += 50 + gap + 30; // Extra gap

            // Logout
            btnDangXuat = CreateNavButton("🚪", "   🚪   Đăng Xuất", startY, AppColors.Red);
            btnDangXuat.IdleColor = Color.FromArgb(60, 243, 139, 168); // Slight red tint
            btnDangXuat.Click += BtnDangXuat_Click;

            // Add controls
            pnlSidebar.Controls.Add(btnToggle);
            pnlSidebar.Controls.Add(pbLogo);
            pnlSidebar.Controls.Add(lblAppName);
            pnlSidebar.Controls.Add(lblWelcome);
            pnlSidebar.Controls.Add(lblMenu);
            pnlSidebar.Controls.Add(btnDashboard);
            pnlSidebar.Controls.Add(btnNhanVien);
            pnlSidebar.Controls.Add(btnBoPhan);
            pnlSidebar.Controls.Add(btnBangLuong);
            pnlSidebar.Controls.Add(btnThongKe);
            pnlSidebar.Controls.Add(btnDangXuat);

            pnlSidebar.Controls.Add(btnDangXuat);

            // Do NOT add to this.Controls here. Will add to TableLayout later.
        }

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

        private RoundedButton CreateNavButton(string icon, string fullText, int y, Color accent)
        {
            var btn = new RoundedButton
            {
                Text = fullText,
                Tag = new MenuButtonData { Icon = icon, FullText = fullText }, // Store Data
                Font = AppFonts.Small,
                Size = new Size(224, 46),
                Location = new Point(18, y),
                IdleColor = Color.FromArgb(20, accent.R, accent.G, accent.B),
                HoverColor = Color.FromArgb(50, accent.R, accent.G, accent.B),
                PressColor = Color.FromArgb(80, accent.R, accent.G, accent.B),
                AccentColor = accent,
                ForeColor = AppColors.Text,
                CornerRadius = 12,
                TextAlign = ContentAlignment.MiddleLeft
            };
            return btn;
        }

        #endregion

        #region Content Area

        private void BuildContentArea()
        {
            // Main content panel that holds child forms
            pnlContent = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = AppColors.Base,
                Padding = new Padding(0) // Remove gap to let child form reach edges
            };

            // Top Header
            pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = AppColors.Mantle,
                Padding = new Padding(24, 0, 24, 0) // Standard gutter
            };

            // Header Title
            lblPageTitle = new Label
            {
                Text = "Dashboard",
                Font = AppFonts.Create(14, FontStyle.Bold),
                ForeColor = AppColors.Text,
                TextAlign = ContentAlignment.MiddleLeft,
                Dock = DockStyle.Left,
                AutoSize = true
            };

            // Date Time
            lblDateTime = new Label
            {
                Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                Font = AppFonts.Tiny,
                ForeColor = AppColors.SubText,
                TextAlign = ContentAlignment.MiddleRight,
                Dock = DockStyle.Right,
                AutoSize = true
            };
            
            // To align vertically
            // We can use a TableLayout inside Header or just set consistent inner padding/margin.
            // But Dock logic is sufficient if labels are AutoSize and we handle Layout event or TextChanged.
            // Label vert alignment is handled by TextAlign only if AutoSize=false and Height=Parent.Height.
            lblPageTitle.AutoSize = false;
            lblPageTitle.Height = 60; // Match header height
            lblPageTitle.Width = 300; // Fixed width for title
            
            lblDateTime.AutoSize = false;
            lblDateTime.Height = 60;
            lblDateTime.Width = 200;

            pnlHeader.Controls.Add(lblPageTitle);
            pnlHeader.Controls.Add(lblDateTime);

            // Add Header to Content Panel
            // Note: We want pnlHeader *inside* pnlContent or *above* pnlContent?
            // Usually Header is part of the layout.
            // If pnlContent is Dock.Fill, we can add Header (Dock.Top) to FormMain (Dock.Fill doesn't work well with other specific docks unless layered correctly).
            // Better: Add pnlHeader to THIS Form (Dock.Top), and pnlSidebar (Dock.Left).
            // Then pnlContent (Dock.Fill) fills the rest.
            // BUT: We want Header to extend over the content area, but NOT cover the Sidebar (usually).
            // Current Sidebar is Dock.Left.
            // So content area is the remaining space.
            
            // Let's add header to a container that also holds content.
            // Let's call it pnlRightSide.
            _pnlRightSide = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = AppColors.Base
            };
            
            _pnlRightSide.Controls.Add(pnlHeader);  // Add Header first -> becomes Index 1
            _pnlRightSide.Controls.Add(pnlContent); // Add Content second -> becomes Index 0
            
            pnlContent.BringToFront(); 
        }

        #endregion

        #region Logic

        private void ToggleSidebar()
        {
            _isCollapsed = !_isCollapsed;

            if (_isCollapsed)
            {
                tblMainLayout.ColumnStyles[0].Width = 70;
                pnlSidebar.Width = 70;
                btnToggle.Location = new Point(15, 10); // Center hamburger
                
                lblAppName.Visible = false;
                lblWelcome.Visible = false;
                lblMenu.Visible = false;
                
                // Adjust logo
                pbLogo.Width = 70;
                pbLogo.Location = new Point(0, 40); 
                pbLogo.Height = 40;
                UpdateButtonState(btnDashboard, true);
                UpdateButtonState(btnNhanVien, true);
                UpdateButtonState(btnBoPhan, true);
                UpdateButtonState(btnBangLuong, true);
                UpdateButtonState(btnThongKe, true);
                UpdateButtonState(btnDangXuat, true);
            }
            else
            {
                tblMainLayout.ColumnStyles[0].Width = 260;
                pnlSidebar.Width = 260;
                btnToggle.Location = new Point(210, 10); // Restore hamburger pos

                lblAppName.Visible = true;
                lblWelcome.Visible = true;
                lblMenu.Visible = true;
                
                // Restore logo
                pbLogo.Width = 260;
                pbLogo.Height = 60;
                pbLogo.Location = new Point(0, 30);

                UpdateButtonState(btnDashboard, false);
                UpdateButtonState(btnNhanVien, false);
                UpdateButtonState(btnBoPhan, false);
                UpdateButtonState(btnBangLuong, false);
                UpdateButtonState(btnThongKe, false);
                UpdateButtonState(btnDangXuat, false);
            }
        }

        private void UpdateButtonState(RoundedButton btn, bool collapsed)
        {
            var data = (MenuButtonData)btn.Tag;
            if (collapsed)
            {
                btn.Text = data.Icon; // Just icon
                btn.Size = new Size(46, 46); // Square
                btn.Location = new Point(12, btn.Location.Y); // 12 + 46 + 12 = 70
                btn.TextAlign = ContentAlignment.MiddleCenter;
                btn.Padding = new Padding(0);
            }
            else
            {
                btn.Text = data.FullText;
                btn.Size = new Size(224, 46);
                btn.Location = new Point(18, btn.Location.Y);
                btn.TextAlign = ContentAlignment.MiddleLeft;
                btn.Padding = new Padding(20, 0, 10, 0); 
            }
        }

        public void OpenChildForm(Form childForm, RoundedButton senderBtn, string title)
        {
            if (_activeForm != null)
            {
                _activeForm.Close();
                _activeForm.Dispose(); // Ensure cleanup
            }

            _activeForm = childForm;
            
            // Style previous button
            if (_currentButton != null)
            {
                // Reset style (using IdleColor logic from RoundedButton default)
                // We rely on RoundedButton logic for normal state.
                // But we can force an "Active" look here if needed.
                // For now, let's just tracking logic.
                var normalData = (MenuButtonData)_currentButton.Tag;
                // Currently RoundedButton doesn't support "Selected" property persistently 
                // unless we subclass it or manually manage colors.
                // We'll leave it as-is for now (hover effects are usually enough), 
                // or we could manually set IdleColor to be brighter.
                // Let's implement a simple "Active" highlight manually:
                 Color baseAccent = _currentButton.AccentColor;
                 _currentButton.IdleColor = Color.FromArgb(20, baseAccent.R, baseAccent.G, baseAccent.B);
            }

            _currentButton = senderBtn;
            
            // Highlight new button
            if (_currentButton != null)
            {
                Color activeAccent = _currentButton.AccentColor;
                _currentButton.IdleColor = Color.FromArgb(60, activeAccent.R, activeAccent.G, activeAccent.B); // Brighter background
            }

            // Setup new form
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
            var result = MessageBox.Show("Bạn đàng muốn đăng xuất khỏi hệ thống?",
                "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            
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
                // Force Vietnamese culture for day names
                var viCulture = new System.Globalization.CultureInfo("vi-VN");
                lblDateTime.Text = DateTime.Now.ToString("dddd, dd/MM/yyyy HH:mm", viCulture);
                
                // Capitalize first letter (e.g., "Thứ Ba")
                if (lblDateTime.Text.Length > 0)
                    lblDateTime.Text = char.ToUpper(lblDateTime.Text[0]) + lblDateTime.Text.Substring(1);
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
