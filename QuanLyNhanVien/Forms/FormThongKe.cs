using System;
using System.Drawing;
using System.Windows.Forms;
using System.Data;
using QuanLyNhanVien.Services;
using QuanLyNhanVien.Infrastructure;

namespace QuanLyNhanVien.Forms
{
    public class FormThongKe : Form
    {
        private ComboBox cboNam;
        private Button btnXem;
        private DataGridView dgv;
        private Label lblTongNam;

        private readonly ThongKeService _service = new ThongKeService();

        public FormThongKe()
        {
            InitializeComponent();
            cboNam.SelectedItem = DateTime.Now.Year;
        }

        private void InitializeComponent()
        {
            this.Text = "Thống Kê Lương Theo Tháng";
            this.BackColor = AppColors.Base;
            this.Padding = new Padding(0);

            // === TOP PANEL ===
            var pnlTop = new Panel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                BackColor = AppColors.Surface0,
                Padding = new Padding(20, 10, 20, 10)
            };

            var flowTop = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true
            };

            var lblNam = new Label
            {
                Text = "Chọn năm:",
                Font = AppFonts.Body,
                ForeColor = AppColors.SubText,
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleLeft,
                Margin = new Padding(0, 8, 10, 0)
            };

            cboNam = new ComboBox
            {
                Font = AppFonts.Small,
                Width = 100,
                DropDownStyle = ComboBoxStyle.DropDownList,
                BackColor = AppColors.InputBg,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Margin = new Padding(0, 5, 20, 0)
            };
            int yr = DateTime.Now.Year;
            for (int y = yr - 5; y <= yr + 1; y++) cboNam.Items.Add(y);

            btnXem = new Button
            {
                Text = "📊 Xem Thống Kê",
                Font = AppFonts.SmallBold,
                Size = new Size(160, 32),
                BackColor = AppColors.Blue,
                ForeColor = AppColors.Base,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Margin = new Padding(0, 0, 20, 0)
            };
            btnXem.FlatAppearance.BorderSize = 0;
            btnXem.Click += BtnXem_Click;

            lblTongNam = new Label
            {
                Text = "",
                Font = AppFonts.BodyBold,
                ForeColor = AppColors.Yellow,
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleLeft,
                Margin = new Padding(0, 8, 0, 0)
            };

            flowTop.Controls.AddRange(new Control[] { lblNam, cboNam, btnXem, lblTongNam });
            pnlTop.Controls.Add(flowTop);
            
            // === MAIN LAYOUT ===
            var mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                BackColor = Color.Transparent
            };
            mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            this.Controls.Add(mainLayout);

            pnlTop.Dock = DockStyle.Fill;
            mainLayout.Controls.Add(pnlTop, 0, 0);

            // === GRID ===
            var pnlGrid = new Panel
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 10, 0, 0), // Automatic gap
                Padding = new Padding(0)
            };

            dgv = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = AppColors.Base,
                BorderStyle = BorderStyle.None,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None,
                RowHeadersVisible = false,
                Font = AppFonts.Small,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = AppColors.Surface0,
                    ForeColor = AppColors.Text,
                    SelectionBackColor = AppColors.Blue,
                    SelectionForeColor = AppColors.Base,
                    Alignment = DataGridViewContentAlignment.MiddleRight,
                    Font = AppFonts.Small
                },
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = AppColors.Mantle,
                    ForeColor = AppColors.Green,
                    Font = AppFonts.SmallBold,
                    Alignment = DataGridViewContentAlignment.MiddleCenter
                },
                EnableHeadersVisualStyles = false,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing,
                ColumnHeadersHeight = 45,
                RowTemplate = { Height = 35 },
                AutoGenerateColumns = false
            };

            dgv.Columns.AddRange(new DataGridViewColumn[]
            {
                new DataGridViewTextBoxColumn { Name = "Thang", DataPropertyName = "Thang", HeaderText = "Tháng", Width = 90, DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleCenter } },
                new DataGridViewTextBoxColumn { Name = "SoNhanVien", DataPropertyName = "SoNhanVien", HeaderText = "Số NV", Width = 110, DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleCenter } },
                new DataGridViewTextBoxColumn { Name = "TongLuong", DataPropertyName = "TongLuong", HeaderText = "Tổng Lương", Width = 140, DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleRight, Format = "N0" } },
                new DataGridViewTextBoxColumn { Name = "TongUng", DataPropertyName = "TongUng", HeaderText = "Tổng Ứng", Width = 120, DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleRight, Format = "N0" } },
                new DataGridViewTextBoxColumn { Name = "TongBHXH", DataPropertyName = "TongBHXH", HeaderText = "BHXH", Width = 110, DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleRight, Format = "N0" } },
                new DataGridViewTextBoxColumn { Name = "TongThue", DataPropertyName = "TongThue", HeaderText = "Tổng Thuế", Width = 120, DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleRight, Format = "N0" } },
                new DataGridViewTextBoxColumn { Name = "TongThucNhan", DataPropertyName = "TongThucNhan", HeaderText = "Tổng Thực Nhận", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill, MinimumWidth = 150, DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleRight, Format = "N0" } }
            });

            foreach (DataGridViewColumn col in dgv.Columns)
            {
                col.SortMode = DataGridViewColumnSortMode.Programmatic;
            }

            GridHelper.FixAlignment(dgv);

            dgv.ColumnHeaderMouseClick += Dgv_Sort;
            dgv.DataBindingComplete += Dgv_DataBindingComplete;

            pnlGrid.Controls.Add(dgv);
            mainLayout.Controls.Add(pnlGrid, 0, 1);
        }

        private void Dgv_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            // GridHelper handles base alignments.
        }

        private void Dgv_Sort(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (dgv.DataSource == null) return;
            string propertyName = dgv.Columns[e.ColumnIndex].DataPropertyName;
            if (string.IsNullOrEmpty(propertyName)) return;

            DataTable dt = (DataTable)dgv.DataSource;
            _sortAscending = !_sortAscending;

            string sortOrder = _sortAscending ? "ASC" : "DESC";
            dt.DefaultView.Sort = $"{propertyName} {sortOrder}";
            dgv.DataSource = dt.DefaultView.ToTable();
        }

        private bool _sortAscending = false;

        private void BtnXem_Click(object sender, EventArgs e)
        {
            if (cboNam.SelectedItem == null) return;
            int nam = (int)cboNam.SelectedItem;

            try
            {
                var result = _service.LayThongKeNam(nam);
                if (!result.Success)
                {
                    MessageBox.Show(result.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var data = result.Data;
                dgv.DataSource = data.ChiTietTheoThang;

                lblTongNam.Text = $"💰 Tổng chi năm {data.Nam}: {data.TongChiNam:N0} ₫";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
