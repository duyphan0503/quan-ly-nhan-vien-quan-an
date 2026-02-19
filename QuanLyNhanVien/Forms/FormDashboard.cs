using System;
using System.Drawing;
using System.Windows.Forms;
using QuanLyNhanVien.Controls;
using QuanLyNhanVien.Services;

namespace QuanLyNhanVien.Forms
{
    public class FormDashboard : Form
    {
        private DashboardCard cardNhanVien;
        private DashboardCard cardBoPhan;
        private DashboardCard cardLuong;
        private PictureBox pbBigIcon;
        private Label lblWelcomeMsg;
        private Label lblHint;

        public FormDashboard()
        {
            InitializeComponent();
            LoadStats();
        }

        private void InitializeComponent()
        {
            this.Text = "Dashboard";
            this.BackColor = AppColors.Base;
            this.FormBorderStyle = FormBorderStyle.None;
            this.Dock = DockStyle.Fill;
            this.Padding = new Padding(0);

            // Responsive Layout using TableLayoutPanel for Cards
            var tableLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 120,
                ColumnCount = 3,
                RowCount = 1,
                BackColor = Color.Transparent
            };
            
            tableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33f));
            tableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33f));
            tableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33f));

            // Initialize Cards
            cardNhanVien = CreateCard("👥", "—", "Tổng nhân viên", AppColors.Green);
            cardBoPhan = CreateCard("🏢", "—", "Bộ phận", AppColors.Blue);
            cardLuong = CreateCard("💰", "—", "Bảng lương tháng này", AppColors.Yellow);

            // Add to Table (Use wrappers for margins if needed, or card padding)
            tableLayout.Controls.Add(cardNhanVien, 0, 0);
            tableLayout.Controls.Add(cardBoPhan, 1, 0);
            tableLayout.Controls.Add(cardLuong, 2, 0);

            // Welcome Section
            var pnlWelcome = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent,
                Padding = new Padding(0, 40, 0, 0)
            };

            pbBigIcon = new PictureBox
            {
                Image = Image.FromFile("Assets/logo.png"),
                SizeMode = PictureBoxSizeMode.Zoom,
                Dock = DockStyle.Top,
                Height = 120
            };

            lblWelcomeMsg = new Label
            {
                Text = "HỆ THỐNG QUẢN LÝ NHÂN VIÊN\nQUÁN ĂN",
                Font = AppFonts.Create(20, FontStyle.Bold),
                ForeColor = AppColors.Text,
                TextAlign = ContentAlignment.TopCenter,
                Dock = DockStyle.Top,
                Height = 80
            };

            lblHint = new Label
            {
                Text = "Chọn chức năng từ menu bên trái để bắt đầu",
                Font = AppFonts.Small,
                ForeColor = AppColors.Overlay,
                TextAlign = ContentAlignment.TopCenter,
                Dock = DockStyle.Top,
                Height = 40
            };

            pnlWelcome.Controls.Add(lblHint);
            pnlWelcome.Controls.Add(lblWelcomeMsg);
            pnlWelcome.Controls.Add(pbBigIcon);
            
            // Bring labels to correct order (since Dock=Top stacks from bottom if added sequentially, need to be careful or use BringToFront/tabindex)
            // Easier: add in reverse order of display if using Dock.Top
            // Or use FlowLayoutPanel.
            
            // Let's use FlowLayoutPanel for Welcome section to be safe
            var flowWelcome = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoSize = true
            };
            // Note: FlowLayoutPanel centering children is tricky. Dock=Top is reliable if order is correct.
            // Order added: Hint, WelcomeMsg, BigIcon.
            // So visuals will be:
            // [BigIcon]
            // [WelcomeMsg]
            // [Hint]
            //
            // If we add Hint first with Dock=Top, it appears at top.
            // Wait, Dock=Top: The last control added is at the top? No, first added is at top?
            // "The last control added is at the bottom of the z-order, but usually visually at top if correct index"
            // Let's explicitly use BringToFront() or correct add order.
            // Correct order for Dock=Top: 
            // 1. Hint (Bottom-most visually)
            // 2. WelcomeMsg 
            // 3. BigIcon (Top-most visually)
            // No, standard Dock=Top stacks them. The *last* added control is closest to the edge?
            // Actually, `Controls.Add` puts it at index 0. Index 0 is top of Z-order.
            // For Dock=Top, higher Z-index (lower child index) means "closer to the top edge".
            // So:
            // Add BigIcon -> It's at Top.
            // Add WelcomeMsg -> It's below BigIcon? No, it pushes BigIcon down? 
            // WinForms Docking: The *last* control added (or BringToFront) takes the edge first.
            // Let's use a Panel with Anchor centering for a simpler "Center Screen" feel.
            
            var centerPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(0, 50, 0, 0)
            };
            centerPanel.Controls.Add(lblHint);
            centerPanel.Controls.Add(lblWelcomeMsg);
            centerPanel.Controls.Add(pbBigIcon);
            
            pbBigIcon.Dock = DockStyle.Top;
            lblWelcomeMsg.Dock = DockStyle.Top;
            lblHint.Dock = DockStyle.Top;
            
            // Reverse order of adding to make them stack Top-down correctly:
            // We want BigIcon at top, then Welcome, then Hint.
            // If we add Hint, then Welcome, then BigIcon...
            // BigIcon (Dock=Top) -> Takes top.
            // Welcome (Dock=Top) -> Takes remaining top (below BigIcon).
            // Hint (Dock=Top) -> Takes remaining top (below Welcome).
            // So we must add BigIcon first? No, we must add them such that they end up in that order.
            // Actually, let's just use simple defined locations within a center panel or resize event to center them.
            // But we requested "Responsive Alignment".
            // Let's stick to the Dock=Top stack, but ensuring `BringToFront` on the top-most one.
            
            // Correct stack logic:
            // pnlWelcome.Controls.Add(lblBigIcon); 
            // pnlWelcome.Controls.Add(lblWelcomeMsg);
            // pnlWelcome.Controls.Add(lblHint);
            // lblHint.SendToBack(); // Push to bottom of stack = last dock?
            // WinForms docking is dependent on ChildIndex. 
            // ChildIndex 0 = Close to edge. 
            // Add(A); Add(B); -> B is index 0. B is top. A is below B.
            // So we want: BigIcon at Top. So it must be last added?
            // Let's try adding Hint, then Welcome, then BigIcon.
            
            pnlWelcome.Controls.Clear();
            pnlWelcome.Controls.Add(lblHint);        // Index 0
            pnlWelcome.Controls.Add(lblWelcomeMsg);  // Index 0 (pushes Hint to 1)
            pnlWelcome.Controls.Add(pbBigIcon);     // Index 0 (pushes others down)
            
            // Result: BigIcon (Top), Welcome (Middle), Hint (Bottom). Correct.

            this.Controls.Add(pnlWelcome);
            this.Controls.Add(tableLayout);
        }

        private DashboardCard CreateCard(string icon, string value, string subtitle, Color accent)
        {
            return new DashboardCard
            {
                Icon = icon,
                Value = value,
                Subtitle = subtitle,
                AccentColor = accent,
                Dock = DockStyle.Fill,
                Margin = new Padding(10) // Internal spacing in TableLayout
            };
        }

        public void LoadStats()
        {
            try
            {
                var service = new DashboardService();
                var data = service.LayThongKe();

                cardNhanVien.Value = data.TongNhanVien.ToString();
                cardBoPhan.Value = data.TongBoPhan.ToString();
                cardLuong.Value = data.BangLuongThangNay.ToString();
            }
            catch
            {
                // fail silently
            }
        }
    }
}
