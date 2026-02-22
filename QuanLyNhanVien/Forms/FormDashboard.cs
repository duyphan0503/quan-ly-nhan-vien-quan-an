using System.Drawing;
using System.Windows.Forms;
using QuanLyNhanVien.Services;

namespace QuanLyNhanVien.Forms
{
    public partial class FormDashboard : Form
    {
        public FormDashboard()
        {
            InitializeComponent();
            ApplyTheme();
            LoadStats();
        }

        private void ApplyTheme()
        {
            this.BackColor = AppColors.Base;

            // Biểu trưng (Logo)
            try
            {
                string logoPath = System.IO.Path.Combine(
                    System.AppDomain.CurrentDomain.BaseDirectory,
                    "Assets",
                    "logo.png"
                );
                if (System.IO.File.Exists(logoPath))
                    pbBigIcon.Image = System.Drawing.Image.FromFile(logoPath);
            }
            catch
            { /* Thất bại ngầm - không báo lỗi lớn hệ thống */
            }

            // Dán nhãn
            lblWelcomeMsg.Font = AppFonts.Create(20, System.Drawing.FontStyle.Bold);
            lblWelcomeMsg.ForeColor = AppColors.Text;
            lblHint.Font = AppFonts.Small;
            lblHint.ForeColor = AppColors.Overlay;

            // Các tấm Thẻ thông tin (Cards)
            cardNhanVien.AccentColor = AppColors.Green;
            cardNhanVien.Icon = AppIcons.UserGroup;

            cardBoPhan.AccentColor = AppColors.Blue;
            cardBoPhan.Icon = AppIcons.BuildingLg;

            cardLuong.AccentColor = AppColors.Yellow;
            cardLuong.Icon = AppIcons.MoneyLg;
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
                // Thất bại ngầm
            }
        }
    }
}
