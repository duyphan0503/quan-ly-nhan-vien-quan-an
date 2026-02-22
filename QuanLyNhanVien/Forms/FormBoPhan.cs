using System;
using System.Windows.Forms;
using QuanLyNhanVien.Services;

namespace QuanLyNhanVien.Forms
{
    public partial class FormBoPhan : Form
    {
        private readonly BoPhanService _service = new BoPhanService();
        private int _selectedId = -1;
        private bool _sortAscending = false;

        public FormBoPhan()
        {
            InitializeComponent();
            ApplyTheme();
            WireEvents();
            LoadData();
        }

        /// <summary>
        /// Ghi đè giao diện chủ đề trực tiếp khi chạy chương trình (màu sắc Theme, bộ chữ Fonts, biểu tượng Icons) đè lên
        /// nền giao diện nguyên bản mặc định sinh ra bởi trình thiết kế Designer (InitializeComponent).
        /// Cơ chế này đảm bảo sự đồng bộ trên thiết kế giao diện thống nhất và tích hợp phông chữ dự phòng tốt qua nền tảng Mono.
        /// </summary>
        private void ApplyTheme()
        {
            // Khung biểu mẫu nền
            this.BackColor = AppColors.Base;

            // Khung tiếp nhận dữ liệu
            this.pnlInput.BackColor = AppColors.Surface0;

            // Dán nhãn (Label)
            this.lblTenBoPhan.Font = AppFonts.Body;
            this.lblTenBoPhan.ForeColor = AppColors.SubText;

            // Hộp nhập chữ (TextBox)
            this.txtTenBoPhan.Font = AppFonts.Body;
            this.txtTenBoPhan.BackColor = AppColors.InputBg;

            // Áp dụng định dạng nút — theo hệ màu chủ đề + biểu tượng icon
            ApplyButtonTheme(btnThem, AppColors.Green, AppIcons.Add);
            ApplyButtonTheme(btnSua, AppColors.Blue, AppIcons.Edit);
            ApplyButtonTheme(btnXoa, AppColors.Red, AppIcons.Delete);
            ApplyButtonTheme(btnLamMoi, AppColors.Yellow, AppIcons.Refresh);

            // Bảng lưới dữ liệu DataGridView
            this.dgv.AutoGenerateColumns = false;
            this.dgv.BackgroundColor = AppColors.Base;
            this.dgv.Font = AppFonts.Body;
            this.dgv.DefaultCellStyle.BackColor = AppColors.Surface0;
            this.dgv.DefaultCellStyle.ForeColor = AppColors.Text;
            this.dgv.DefaultCellStyle.SelectionBackColor = AppColors.Blue;
            this.dgv.DefaultCellStyle.SelectionForeColor = AppColors.Base;
            this.dgv.DefaultCellStyle.Font = AppFonts.Body;
            this.dgv.ColumnHeadersDefaultCellStyle.BackColor = AppColors.Mantle;
            this.dgv.ColumnHeadersDefaultCellStyle.ForeColor = AppColors.Green;
            this.dgv.ColumnHeadersDefaultCellStyle.Font = AppFonts.BodyBold;

            Infrastructure.GridHelper.FixAlignment(dgv);
        }

        private void ApplyButtonTheme(
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
                btn.TextImageRelation = TextImageRelation.ImageBeforeText;
            }
        }

        private void WireEvents()
        {
            this.btnThem.Click += BtnThem_Click;
            this.btnSua.Click += BtnSua_Click;
            this.btnXoa.Click += BtnXoa_Click;
            this.btnLamMoi.Click += (s, e) => LamMoi();
            this.dgv.ColumnHeaderMouseClick += Dgv_Sort;
            this.dgv.CellClick += Dgv_CellClick;
        }

        private void Dgv_Sort(object sender, DataGridViewCellMouseEventArgs e)
        {
            string propertyName = dgv.Columns[e.ColumnIndex].DataPropertyName;
            if (string.IsNullOrEmpty(propertyName))
                return;

            var list = (System.Collections.Generic.List<QuanLyNhanVien.Models.BoPhan>)
                dgv.DataSource;
            if (list == null)
                return;

            _sortAscending = !_sortAscending;
            var prop = typeof(QuanLyNhanVien.Models.BoPhan).GetProperty(propertyName);
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

        private void LoadData()
        {
            var list = _service.LayTatCa();
            dgv.DataSource = null;
            dgv.DataSource = list;
        }

        private void Dgv_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;
            var row = dgv.Rows[e.RowIndex];
            _selectedId = (int)row.Cells["colMaBoPhan"].Value;
            txtTenBoPhan.Text = row.Cells["colTenBoPhan"].Value?.ToString();
        }

        private void BtnThem_Click(object sender, EventArgs e)
        {
            try
            {
                var result = _service.ThemBoPhan(txtTenBoPhan.Text);
                if (result.Success)
                {
                    MessageBox.Show(
                        result.Message,
                        "Thành công",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                    LamMoi();
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
                var result = _service.CapNhatBoPhan(_selectedId, txtTenBoPhan.Text);
                if (result.Success)
                {
                    MessageBox.Show(
                        result.Message,
                        "Thành công",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                    LamMoi();
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
                    "Vui lòng chọn bộ phận cần xoá!",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            var confirm = MessageBox.Show(
                "Bạn có chắc muốn xoá bộ phận này?",
                "Xác nhận",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );
            if (confirm != DialogResult.Yes)
                return;

            try
            {
                var result = _service.XoaBoPhan(_selectedId);
                if (result.Success)
                {
                    MessageBox.Show(
                        result.Message,
                        "Thành công",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                    LamMoi();
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

        private void LamMoi()
        {
            _selectedId = -1;
            txtTenBoPhan.Clear();
            LoadData();
        }
    }
}
