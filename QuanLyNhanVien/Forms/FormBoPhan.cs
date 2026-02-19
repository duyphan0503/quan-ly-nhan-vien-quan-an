using System;
using System.Drawing;
using System.Windows.Forms;
using QuanLyNhanVien.Services;
using QuanLyNhanVien.Infrastructure;

namespace QuanLyNhanVien.Forms
{
    public class FormBoPhan : Form
    {
        private DataGridView dgv;
        private TextBox txtTenBoPhan;
        private Button btnThem;
        private Button btnSua;
        private Button btnXoa;
        private Button btnLamMoi;

        private readonly BoPhanService _service = new BoPhanService();
        private int _selectedId = -1;

        public FormBoPhan()
        {
            InitializeComponent();
            LoadData();
        }

        private void InitializeComponent()
        {
            this.Text = "Quản Lý Bộ Phận – Chức Vụ";
            this.BackColor = AppColors.Base;
            this.Padding = new Padding(0);

            // === INPUT AREA ===
            var pnlInput = new Panel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                BackColor = AppColors.Surface0,
                Padding = new Padding(20, 10, 20, 10)
            };

            // Input Row using Table
            var table = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                ColumnCount = 2,
                RowCount = 1,
                AutoSize = true,
                Padding = new Padding(0, 0, 0, 10)
            };
            table.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            var lblTen = new Label
            {
                Text = "Tên bộ phận:",
                Font = AppFonts.Body,
                ForeColor = AppColors.SubText,
                AutoSize = true,
                Anchor = AnchorStyles.Left,
                Margin = new Padding(0, 0, 15, 0)
            };

            txtTenBoPhan = new TextBox
            {
                Font = AppFonts.Body,
                BackColor = AppColors.InputBg,
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Dock = DockStyle.Fill
            };
            
            table.Controls.Add(lblTen, 0, 0);
            table.Controls.Add(txtTenBoPhan, 1, 0);
            
            // Buttons Row using Flow
            var flowActions = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                FlowDirection = FlowDirection.LeftToRight,
                AutoSize = true
            };

            btnThem = CreateButton("➕ Thêm", AppColors.Green);
            btnThem.Click += BtnThem_Click;

            btnSua = CreateButton("✏️ Sửa", AppColors.Blue);
            btnSua.Click += BtnSua_Click;

            btnXoa = CreateButton("🗑️ Xoá", AppColors.Red);
            btnXoa.Click += BtnXoa_Click;

            btnLamMoi = CreateButton("🔄 Làm mới", AppColors.Yellow);
            btnLamMoi.Click += (s, e) => LamMoi();

            flowActions.Controls.AddRange(new Control[] { btnThem, btnSua, btnXoa, btnLamMoi });

            // Add to pnlInput (Flow first, Table second means Flow is below Table because both are Dock=Top? No.)
            // Index 0 is Top.
            // We want Table Top, Flow Bottom.
            // So Add Flow (0), then input Table (0 pushes Flow down).
            pnlInput.Controls.Add(flowActions);
            pnlInput.Controls.Add(table);

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

            pnlInput.Dock = DockStyle.Fill;
            mainLayout.Controls.Add(pnlInput, 0, 0);

            // === GRID AREA ===
            var pnlGrid = new Panel
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 10, 0, 0), // Automatic gap below Input Area
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
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None,
                RowHeadersVisible = false,
                Font = AppFonts.Body,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = AppColors.Surface0,
                    ForeColor = AppColors.Text,
                    SelectionBackColor = AppColors.Blue,
                    SelectionForeColor = AppColors.Base,
                    Font = AppFonts.Body
                },
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = AppColors.Mantle,
                    ForeColor = AppColors.Green,
                    Font = AppFonts.BodyBold,
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
                new DataGridViewTextBoxColumn 
                { 
                    Name = "MaBoPhan", 
                    DataPropertyName = "MaBoPhan", 
                    HeaderText = "Mã Bộ Phận", 
                    Width = 150,
                    DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleCenter }
                },
                new DataGridViewTextBoxColumn 
                { 
                    Name = "TenBoPhan", 
                    DataPropertyName = "TenBoPhan", 
                    HeaderText = "Tên Bộ Phận", 
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                    DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleLeft }
                }
            });

            foreach (DataGridViewColumn col in dgv.Columns)
            {
                col.SortMode = DataGridViewColumnSortMode.Programmatic;
            }

            GridHelper.FixAlignment(dgv);

            dgv.ColumnHeaderMouseClick += Dgv_Sort;
            dgv.CellClick += Dgv_CellClick;

            pnlGrid.Controls.Add(dgv);
            mainLayout.Controls.Add(pnlGrid, 0, 1);
        }

        private Button CreateButton(string text, Color bg)
        {
            var btn = new Button
            {
                Text = text,
                Font = AppFonts.TinyBold,
                Size = new Size(110, 36),
                BackColor = bg,
                ForeColor = AppColors.Base,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Margin = new Padding(0, 0, 10, 0)
            };
            btn.FlatAppearance.BorderSize = 0;
            return btn;
        }

        private void Dgv_Sort(object sender, DataGridViewCellMouseEventArgs e)
        {
            string propertyName = dgv.Columns[e.ColumnIndex].DataPropertyName;
            if (string.IsNullOrEmpty(propertyName)) return;

            var list = (System.Collections.Generic.List<QuanLyNhanVien.Models.BoPhan>)dgv.DataSource;
            if (list == null) return;

            _sortAscending = !_sortAscending;
            var prop = typeof(QuanLyNhanVien.Models.BoPhan).GetProperty(propertyName);
            if (prop == null) return;

            if (_sortAscending)
                list.Sort((a, b) => System.Collections.Comparer.Default.Compare(prop.GetValue(a), prop.GetValue(b)));
            else
                list.Sort((a, b) => System.Collections.Comparer.Default.Compare(prop.GetValue(b), prop.GetValue(a)));

            dgv.DataSource = null;
            dgv.DataSource = list;
        }

        private bool _sortAscending = false;

        private void LoadData()
        {
            var list = _service.LayTatCa();
            dgv.DataSource = null;
            dgv.DataSource = list;
        }

        private void Dgv_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var row = dgv.Rows[e.RowIndex];
            _selectedId = (int)row.Cells["MaBoPhan"].Value;
            txtTenBoPhan.Text = row.Cells["TenBoPhan"].Value?.ToString();
        }

        private void BtnThem_Click(object sender, EventArgs e)
        {
            try
            {
                var result = _service.ThemBoPhan(txtTenBoPhan.Text);
                if (result.Success)
                {
                    MessageBox.Show("✅ " + result.Message, "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LamMoi();
                }
                else
                {
                    MessageBox.Show(result.Message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnSua_Click(object sender, EventArgs e)
        {
            try
            {
                var result = _service.CapNhatBoPhan(_selectedId, txtTenBoPhan.Text);
                if (result.Success)
                {
                    MessageBox.Show("✅ " + result.Message, "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LamMoi();
                }
                else
                {
                    MessageBox.Show(result.Message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnXoa_Click(object sender, EventArgs e)
        {
            if (_selectedId < 0)
            {
                MessageBox.Show("Vui lòng chọn bộ phận cần xoá!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var confirm = MessageBox.Show("Bạn có chắc muốn xoá bộ phận này?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes) return;

            try
            {
                var result = _service.XoaBoPhan(_selectedId);
                if (result.Success)
                {
                    MessageBox.Show("✅ " + result.Message, "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LamMoi();
                }
                else
                {
                    MessageBox.Show(result.Message, "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
