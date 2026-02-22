using System;
using System.Windows.Forms;
using QuanLyNhanVien.Models;
using QuanLyNhanVien.Services;

namespace QuanLyNhanVien.Forms
{
    public partial class FormNhanVien : Form
    {
        private readonly NhanVienService _service = new NhanVienService();
        private readonly BoPhanService _boPhanService = new BoPhanService();
        private int _selectedId = -1;
        private bool _sortAscending = false;

        public FormNhanVien()
        {
            InitializeComponent();
            ApplyTheme();
            WireEvents();
            LoadBoPhan();
            LoadData();
        }

        private void ApplyTheme()
        {
            this.BackColor = AppColors.Base;
            this.pnlInput.BackColor = AppColors.Surface0;

            // Nhãn dán (Labels)
            foreach (System.Windows.Forms.Control c in tableInput.Controls)
            {
                if (c is System.Windows.Forms.Label lbl)
                {
                    lbl.Font = AppFonts.Small;
                    lbl.ForeColor = AppColors.SubText;
                }
                else if (c is System.Windows.Forms.TextBox txt)
                {
                    txt.Font = AppFonts.Body;
                    txt.BackColor = AppColors.InputBg;
                }
                else if (c is System.Windows.Forms.ComboBox cbo)
                {
                    cbo.Font = AppFonts.Body;
                    cbo.BackColor = AppColors.InputBg;
                }
            }

            lblSearch.Font = AppFonts.Small;
            lblSearch.ForeColor = AppColors.SubText;
            txtTimKiem.Font = AppFonts.Body;
            txtTimKiem.BackColor = AppColors.InputBg;

            // Các nút bấm (Buttons)
            ApplyBtnTheme(btnThem, AppColors.Green, AppIcons.Add);
            ApplyBtnTheme(btnSua, AppColors.Blue, AppIcons.Edit);
            ApplyBtnTheme(btnXoa, AppColors.Red, AppIcons.Delete);
            ApplyBtnTheme(btnLamMoi, AppColors.Yellow, AppIcons.Refresh);

            // Bảng hiện thị lưới dữ liệu (Grid control)
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
            dgv.ColumnHeadersDefaultCellStyle.Font = AppFonts.SmallBold;

            Infrastructure.GridHelper.FixAlignment(dgv);

            if (dgv.Columns.Contains("colLuongCoBan"))
            {
                dgv.Columns["colLuongCoBan"].DefaultCellStyle.Format = "N0";
                dgv.Columns["colLuongCoBan"].DefaultCellStyle.Alignment = System
                    .Windows
                    .Forms
                    .DataGridViewContentAlignment
                    .MiddleRight;
            }
        }

        private void ApplyBtnTheme(
            System.Windows.Forms.Button btn,
            System.Drawing.Color bg,
            System.Drawing.Image icon
        )
        {
            btn.BackColor = bg;
            btn.ForeColor = AppColors.Base;
            btn.Font = AppFonts.TinyBold;
            if (icon != null)
            {
                btn.Image = icon;
                btn.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
                btn.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            }
        }

        private void WireEvents()
        {
            btnThem.Click += BtnThem_Click;
            btnSua.Click += BtnSua_Click;
            btnXoa.Click += BtnXoa_Click;
            btnLamMoi.Click += (s, e) =>
            {
                LamMoi();
                LoadData();
            };
            txtTimKiem.KeyDown += (s, e) =>
            {
                if (e.KeyCode == System.Windows.Forms.Keys.Enter)
                    TimKiem();
            };
            dgv.ColumnHeaderMouseClick += Dgv_Sort;
            dgv.CellClick += Dgv_CellClick;
        }

        private void LoadBoPhan()
        {
            var list = _boPhanService.LayTatCa();
            cboBoPhan.DataSource = list;
            cboBoPhan.DisplayMember = "TenBoPhan";
            cboBoPhan.ValueMember = "MaBoPhan";
        }

        private void LoadData()
        {
            var list = _service.LayTatCa();
            dgv.DataSource = null;
            dgv.DataSource = list;
        }

        private void Dgv_Sort(object sender, DataGridViewCellMouseEventArgs e)
        {
            string propertyName = dgv.Columns[e.ColumnIndex].DataPropertyName;
            if (string.IsNullOrEmpty(propertyName))
                return;

            var list = (System.Collections.Generic.List<NhanVien>)dgv.DataSource;
            if (list == null)
                return;

            _sortAscending = !_sortAscending;

            var prop = typeof(NhanVien).GetProperty(propertyName);
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

        private void Dgv_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;
            var row = dgv.Rows[e.RowIndex];
            _selectedId = (int)row.Cells["colMaNV"].Value;
            txtHoTen.Text = row.Cells["colHoTen"].Value?.ToString();
            txtChucVu.Text = row.Cells["colChucVu"].Value?.ToString();
            cboBoPhan.SelectedValue = row.Cells["colMaBoPhan"].Value;
            txtLuongCoBan.Text = ((decimal)row.Cells["colLuongCoBan"].Value).ToString("0");
            cboTrangThai.SelectedItem = row.Cells["colTrangThai"].Value?.ToString();
        }

        private NhanVien LayThongTuInput()
        {
            decimal luong;
            if (!decimal.TryParse(txtLuongCoBan.Text, out luong))
                luong = -1;

            return new NhanVien
            {
                MaNV = _selectedId,
                HoTen = txtHoTen.Text?.Trim(),
                ChucVu = txtChucVu.Text?.Trim(),
                MaBoPhan = cboBoPhan.SelectedValue != null ? (int)cboBoPhan.SelectedValue : 0,
                LuongCoBan = luong,
                TrangThai = cboTrangThai.SelectedItem?.ToString() ?? "Đang làm",
            };
        }

        private void BtnThem_Click(object sender, EventArgs e)
        {
            try
            {
                var nv = LayThongTuInput();
                var result = _service.ThemNhanVien(nv);

                if (result.Success)
                {
                    MessageBox.Show(
                        result.Message,
                        "Thành công",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                    LamMoi();
                    LoadData();
                }
                else
                {
                    MessageBox.Show(
                        result.Message,
                        "Thông báo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                }
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

        private void BtnSua_Click(object sender, EventArgs e)
        {
            try
            {
                var nv = LayThongTuInput();
                var result = _service.CapNhatNhanVien(nv);

                if (result.Success)
                {
                    MessageBox.Show(
                        result.Message,
                        "Thành công",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                    LamMoi();
                    LoadData();
                }
                else
                {
                    MessageBox.Show(
                        result.Message,
                        "Thông báo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                }
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

        private void BtnXoa_Click(object sender, EventArgs e)
        {
            if (_selectedId < 0)
            {
                MessageBox.Show(
                    "Vui lòng chọn nhân viên cần xoá!",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            var confirm = MessageBox.Show(
                "Bạn có chắc muốn xoá nhân viên này?",
                "Xác nhận",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );
            if (confirm != DialogResult.Yes)
                return;

            try
            {
                var result = _service.XoaNhanVien(_selectedId);
                if (result.Success)
                {
                    MessageBox.Show(
                        result.Message,
                        "Thành công",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                    LamMoi();
                    LoadData();
                }
                else
                {
                    MessageBox.Show(
                        result.Message,
                        "Cảnh báo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                }
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

        private void TimKiem()
        {
            string kw = txtTimKiem.Text.Trim();
            var list = _service.TimKiem(kw);
            dgv.DataSource = null;
            dgv.DataSource = list;
        }

        private void LamMoi()
        {
            _selectedId = -1;
            txtHoTen.Clear();
            txtChucVu.Clear();
            txtLuongCoBan.Clear();
            cboTrangThai.SelectedIndex = 0;
            if (cboBoPhan.Items.Count > 0)
                cboBoPhan.SelectedIndex = 0;
            txtTimKiem.Clear();
        }
    }
}
