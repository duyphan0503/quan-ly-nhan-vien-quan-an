using System;
using System.Collections.Generic;
using System.Windows.Forms;
using QuanLyNhanVien.Infrastructure;
using QuanLyNhanVien.Models;
using QuanLyNhanVien.Services;

namespace QuanLyNhanVien.Forms
{
    public partial class FormBangLuong : Form
    {
        private readonly BangLuongService _service = new BangLuongService();
        private decimal _luongCoBan = 0;
        private bool _sortAscending = false;

        public FormBangLuong()
        {
            InitializeComponent();
            ApplyTheme();
            WireEvents();
            PopulateCombos();
            LoadNhanVien();
        }

        private void ApplyTheme()
        {
            this.BackColor = AppColors.Base;
            pnlInput.BackColor = AppColors.Surface0;

            // Nhãn ở dòng 1 & 2
            foreach (Control c in row1.Controls)
            {
                if (c is Label lbl)
                {
                    lbl.Font = AppFonts.Small;
                    lbl.ForeColor = AppColors.SubText;
                }
                if (c is ComboBox cbo)
                {
                    cbo.Font = AppFonts.Body;
                    cbo.BackColor = AppColors.InputBg;
                }
            }
            foreach (Control c in row2.Controls)
            {
                if (c is Label lbl && lbl != lblLuongCoBan)
                {
                    lbl.Font = AppFonts.Small;
                    lbl.ForeColor = AppColors.SubText;
                }
                if (c is TextBox txt)
                {
                    txt.Font = AppFonts.Body;
                    txt.BackColor = AppColors.InputBg;
                }
            }

            lblLuongCoBan.Font = AppFonts.BodyBold;
            lblLuongCoBan.ForeColor = AppColors.Yellow;

            // Khu vực kết quả
            grpResult.BackColor = AppColors.Mantle;
            lblResHeader.Font = AppFonts.SmallBold;
            lblResHeader.ForeColor = AppColors.Lavender;
            lblResLuongTheoCongVal.Font = AppFonts.SmallBold;
            lblResBHXHVal.Font = AppFonts.SmallBold;
            lblResThueVal.Font = AppFonts.SmallBold;
            lblResTongThucNhanVal.Font = AppFonts.XLargeBold;
            lblResTongThucNhanVal.ForeColor = AppColors.Green;

            // Nút bấm
            ApplyBtnTheme(btnTinhLuong, AppColors.Blue, AppIcons.Calculator);
            ApplyBtnTheme(btnLuu, AppColors.Green, AppIcons.Save);
            ApplyBtnTheme(btnXemDS, AppColors.Yellow, AppIcons.List);
            ApplyBtnTheme(btnXuatExcel, AppColors.Teal, null);
            ApplyBtnTheme(btnXuatTatCa, AppColors.Mauve, null);

            // Bảng dữ liệu (Grid)
            dgv.AutoGenerateColumns = false;
            dgv.BackgroundColor = AppColors.Base;
            dgv.Font = AppFonts.Small;
            dgv.DefaultCellStyle.BackColor = AppColors.Surface0;
            dgv.DefaultCellStyle.ForeColor = AppColors.Text;
            dgv.DefaultCellStyle.SelectionBackColor = AppColors.Blue;
            dgv.DefaultCellStyle.SelectionForeColor = AppColors.Base;
            dgv.DefaultCellStyle.Font = AppFonts.Small;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = AppColors.Mantle;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = AppColors.Green;
            dgv.ColumnHeadersDefaultCellStyle.Font = AppFonts.TinyBold;

            Infrastructure.GridHelper.FixAlignment(dgv);
        }

        private void ApplyBtnTheme(Button btn, System.Drawing.Color bg, System.Drawing.Image icon)
        {
            btn.BackColor = bg;
            btn.ForeColor = AppColors.Base;
            btn.Font = AppFonts.TinyBold;
            if (icon != null)
            {
                btn.Image = icon;
                btn.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
                btn.TextImageRelation = TextImageRelation.ImageBeforeText;
            }
        }

        private void WireEvents()
        {
            cboNhanVien.SelectedIndexChanged += CboNhanVien_Changed;
            btnTinhLuong.Click += BtnTinhLuong_Click;
            btnLuu.Click += BtnLuu_Click;
            btnXemDS.Click += BtnXemDS_Click;
            dgv.ColumnHeaderMouseClick += Dgv_Sort;
            dgv.DataBindingComplete += Dgv_DataBindingComplete;
            btnXuatExcel.Click += BtnXuatExcel_Click;
            btnXuatTatCa.Click += BtnXuatTatCa_Click;
        }

        private void PopulateCombos()
        {
            for (int i = 1; i <= 12; i++)
                cboThang.Items.Add(i);

            int curYear = DateTime.Now.Year;
            for (int y = curYear - 5; y <= curYear + 1; y++)
                cboNam.Items.Add(y);

            cboThang.SelectedIndex = DateTime.Now.Month - 1;
            cboNam.SelectedItem = curYear;
        }

        // Các hàm xử lý nghiệp vụ logic
        private void LoadNhanVien()
        {
            var list = _service.LayDanhSachNhanVien();
            cboNhanVien.DataSource = list;
            cboNhanVien.DisplayMember = "HoTen";
            cboNhanVien.ValueMember = "MaNV";
        }

        private void Dgv_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            // GridHelper đã xử lý sẵn các trường hợp căn lề cơ bản.
        }

        private void Dgv_Sort(object sender, DataGridViewCellMouseEventArgs e)
        {
            string propertyName = dgv.Columns[e.ColumnIndex].DataPropertyName;
            if (string.IsNullOrEmpty(propertyName))
                return;

            var list = (System.Collections.Generic.List<BangLuong>)dgv.DataSource;
            if (list == null)
                return;

            _sortAscending = !_sortAscending;
            var prop = typeof(BangLuong).GetProperty(propertyName);
            if (prop == null)
                return;

            if (_sortAscending)
                list.Sort(
                    (a, b) =>
                        System.Collections.Comparer.Default.Compare(
                            prop.GetValue(a),
                            prop.GetValue(b)
                        )
                );
            else
                list.Sort(
                    (a, b) =>
                        System.Collections.Comparer.Default.Compare(
                            prop.GetValue(b),
                            prop.GetValue(a)
                        )
                );

            dgv.DataSource = null;
            dgv.DataSource = list;
        }

        private void CboNhanVien_Changed(object sender, EventArgs e)
        {
            if (cboNhanVien.SelectedItem is NhanVien nv)
            {
                _luongCoBan = nv.LuongCoBan;
                lblLuongCoBan.Text = nv.LuongCoBan.ToString("N0") + " ₫";
            }
        }

        private void BtnTinhLuong_Click(object sender, EventArgs e)
        {
            decimal ngayCong;
            if (!decimal.TryParse(txtNgayCong.Text, out ngayCong))
            {
                MessageBox.Show(
                    "Ngày công không hợp lệ!",
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            decimal tienUng;
            if (!decimal.TryParse(txtTienUng.Text, out tienUng))
            {
                MessageBox.Show(
                    "Tiền ứng không hợp lệ!",
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            var result = _service.TinhLuong(_luongCoBan, ngayCong, tienUng);
            if (!result.Success)
            {
                MessageBox.Show(
                    result.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            var kq = result.Data;
            lblResLuongTheoCongVal.Text = kq.LuongTheoCong.ToString("N0") + " ₫";
            lblResBHXHVal.Text = kq.BHXH.ToString("N0") + " ₫";
            lblResThueVal.Text = kq.Thue.ToString("N0") + " ₫";
            lblResTongThucNhanVal.Text = kq.TongThucNhan.ToString("N0") + " ₫";
        }

        private void BtnLuu_Click(object sender, EventArgs e)
        {
            if (
                cboNhanVien.SelectedValue == null
                || cboThang.SelectedItem == null
                || cboNam.SelectedItem == null
            )
            {
                MessageBox.Show(
                    "Vui lòng chọn đầy đủ thông tin!",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            decimal ngayCong;
            if (!decimal.TryParse(txtNgayCong.Text, out ngayCong))
            {
                MessageBox.Show(
                    "Ngày công không hợp lệ!",
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            decimal tienUng;
            if (!decimal.TryParse(txtTienUng.Text, out tienUng))
                tienUng = 0;

            try
            {
                int maNV = (int)cboNhanVien.SelectedValue;
                int thang = (int)cboThang.SelectedItem;
                int nam = (int)cboNam.SelectedItem;

                var result = _service.LuuBangLuong(
                    maNV,
                    thang,
                    nam,
                    _luongCoBan,
                    ngayCong,
                    tienUng
                );

                if (result.Success)
                    MessageBox.Show(
                        result.Message,
                        "Thành công",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                else
                    MessageBox.Show(
                        result.Message,
                        "Lỗi",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Lỗi: " + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void BtnXemDS_Click(object sender, EventArgs e)
        {
            if (cboThang.SelectedItem == null || cboNam.SelectedItem == null)
                return;
            int thang = (int)cboThang.SelectedItem;
            int nam = (int)cboNam.SelectedItem;

            var list = _service.LayTheoThangNam(thang, nam);
            dgv.DataSource = null;
            dgv.DataSource = list;
        }

        // ══════════════════════════════════════════════
        //  XUẤT EXCEL PHIẾU LƯƠNG
        // ══════════════════════════════════════════════

        /// <summary>
        /// Xuất 1 phiếu lương của nhân viên đang chọn trên bảng.
        /// </summary>
        private void BtnXuatExcel_Click(object sender, EventArgs e)
        {
            if (dgv.CurrentRow == null || dgv.DataSource == null)
            {
                MessageBox.Show(
                    "Vui lòng bấm \"Xem DS Tháng\" và chọn 1 phiếu lương trước!",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
                return;
            }

            var list = (List<BangLuong>)dgv.DataSource;
            int idx = dgv.CurrentRow.Index;
            if (idx < 0 || idx >= list.Count)
                return;

            var bl = list[idx];

            using (var sfd = new SaveFileDialog())
            {
                sfd.Title = "Lưu phiếu lương Excel";
                sfd.Filter = "Excel 2007+ (*.xlsx)|*.xlsx";
                sfd.FileName =
                    $"PhieuLuong_{bl.HoTen?.Replace(" ", "_")}_{bl.Thang:D2}_{bl.Nam}.xlsx";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        ExcelExporter.ExportMotPhieu(bl, sfd.FileName);
                        MessageBox.Show(
                            $"Xuất phiếu lương thành công!\n{sfd.FileName}",
                            "Thành công",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information
                        );
                        // Mở file vừa xuất
                        System.Diagnostics.Process.Start(sfd.FileName);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(
                            "Lỗi xuất Excel: " + ex.Message,
                            "Lỗi",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
                    }
                }
            }
        }

        /// <summary>
        /// Xuất toàn bộ phiếu lương trong tháng đang hiển thị trên bảng.
        /// </summary>
        private void BtnXuatTatCa_Click(object sender, EventArgs e)
        {
            if (cboThang.SelectedItem == null || cboNam.SelectedItem == null)
            {
                MessageBox.Show(
                    "Vui lòng chọn Tháng và Năm!",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            int thang = (int)cboThang.SelectedItem;
            int nam = (int)cboNam.SelectedItem;

            var list = _service.LayTheoThangNam(thang, nam);
            if (list == null || list.Count == 0)
            {
                MessageBox.Show(
                    $"Không có dữ liệu lương tháng {thang}/{nam}.",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
                return;
            }

            using (var sfd = new SaveFileDialog())
            {
                sfd.Title = "Lưu tất cả phiếu lương tháng";
                sfd.Filter = "Excel 2007+ (*.xlsx)|*.xlsx";
                sfd.FileName = $"PhieuLuong_Thang{thang:D2}_{nam}_TatCa.xlsx";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        ExcelExporter.ExportTatCaPhieu(list, thang, nam, sfd.FileName);
                        MessageBox.Show(
                            $"Xuất thành công {list.Count} phiếu lương!\n{sfd.FileName}",
                            "Thành công",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information
                        );
                        System.Diagnostics.Process.Start(sfd.FileName);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(
                            "Lỗi xuất Excel: " + ex.Message,
                            "Lỗi",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
                    }
                }
            }
        }
    }
}
