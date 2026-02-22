using System;
using System.Data;
using System.Windows.Forms;
using QuanLyNhanVien.Services;

namespace QuanLyNhanVien.Forms
{
    public partial class FormThongKe : Form
    {
        private readonly ThongKeService _service = new ThongKeService();
        private bool _sortAscending = false;

        public FormThongKe()
        {
            InitializeComponent();
            ApplyTheme();
            WireEvents();
            PopulateYears();
        }

        private void ApplyTheme()
        {
            this.BackColor = AppColors.Base;
            pnlTop.BackColor = AppColors.Surface0;

            lblNam.Font = AppFonts.Body;
            lblNam.ForeColor = AppColors.SubText;

            cboNam.Font = AppFonts.Small;
            cboNam.BackColor = AppColors.InputBg;

            btnXem.BackColor = AppColors.Blue;
            btnXem.ForeColor = AppColors.Base;
            btnXem.Font = AppFonts.SmallBold;
            btnXem.Image = AppIcons.Chart;
            btnXem.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            btnXem.TextImageRelation = TextImageRelation.ImageBeforeText;

            lblTongNam.Font = AppFonts.BodyBold;
            lblTongNam.ForeColor = AppColors.Yellow;

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

            string[] moneyCols =
            {
                "colTongLuong",
                "colTongUng",
                "colTongBHXH",
                "colTongThue",
                "colTongThucNhan",
            };
            foreach (var c in moneyCols)
            {
                if (dgv.Columns.Contains(c))
                {
                    dgv.Columns[c].DefaultCellStyle.Format = "N0";
                    dgv.Columns[c].DefaultCellStyle.Alignment = System
                        .Windows
                        .Forms
                        .DataGridViewContentAlignment
                        .MiddleRight;
                }
            }
        }

        private void WireEvents()
        {
            btnXem.Click += BtnXem_Click;
            dgv.ColumnHeaderMouseClick += Dgv_Sort;
            dgv.DataBindingComplete += Dgv_DataBindingComplete;
        }

        private void PopulateYears()
        {
            int yr = DateTime.Now.Year;
            for (int y = yr - 5; y <= yr + 1; y++)
                cboNam.Items.Add(y);
            cboNam.SelectedItem = yr;
        }

        private void Dgv_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            // Tệp GridHelper sẽ xử lý các công việc liên quan đến căn lề cốt lõi.
        }

        private void Dgv_Sort(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (dgv.DataSource == null)
                return;
            string propertyName = dgv.Columns[e.ColumnIndex].DataPropertyName;
            if (string.IsNullOrEmpty(propertyName))
                return;

            DataTable dt = (DataTable)dgv.DataSource;
            _sortAscending = !_sortAscending;

            string sortOrder = _sortAscending ? "ASC" : "DESC";
            dt.DefaultView.Sort = $"{propertyName} {sortOrder}";
            dgv.DataSource = dt.DefaultView.ToTable();
        }

        private void BtnXem_Click(object sender, EventArgs e)
        {
            if (cboNam.SelectedItem == null)
                return;
            int nam = (int)cboNam.SelectedItem;

            try
            {
                var result = _service.LayThongKeNam(nam);
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

                var data = result.Data;
                dgv.DataSource = data.ChiTietTheoThang;

                lblTongNam.Text = $"Tổng chi năm {data.Nam}: {data.TongChiNam:N0} ₫";
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
    }
}
