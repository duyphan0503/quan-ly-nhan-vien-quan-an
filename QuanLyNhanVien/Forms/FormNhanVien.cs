using System;
using System.Drawing;
using System.Windows.Forms;
using QuanLyNhanVien.Models;
using QuanLyNhanVien.Services;
using QuanLyNhanVien.Infrastructure;

namespace QuanLyNhanVien.Forms
{
    public class FormNhanVien : Form
    {
        private DataGridView dgv;
        private TextBox txtHoTen;
        private TextBox txtChucVu;
        private ComboBox cboBoPhan;
        private TextBox txtLuongCoBan;
        private ComboBox cboTrangThai;
        private TextBox txtTimKiem;
        private Button btnThem;
        private Button btnSua;
        private Button btnXoa;
        private Button btnLamMoi;

        private readonly NhanVienService _service = new NhanVienService();
        private readonly BoPhanService _boPhanService = new BoPhanService();
        private int _selectedId = -1;

        public FormNhanVien()
        {
            InitializeComponent();
            LoadBoPhan();
            LoadData();
        }

        private void InitializeComponent()
        {
            this.Text = "Quản Lý Nhân Viên";
            this.Size = new Size(1000, 600);
            this.BackColor = AppColors.Base;
            this.Padding = new Padding(0); // Add outer padding

            // === INPUT AREA (Top) ===
            var pnlInput = new Panel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                BackColor = AppColors.Surface0, 
                Padding = new Padding(20, 10, 20, 10)
            };

            // Table for Inputs
            var table = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                ColumnCount = 4,
                RowCount = 3,
                AutoSize = true,
                Padding = new Padding(0, 0, 0, 10)
            };
            
            // Column styles: Label (Auto), Input (50%), Label (Auto), Input (50%)
            table.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            table.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            
            // Row styles
            table.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            table.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            table.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Buttons row

            // --- Row 0 ---
            // Ho Ten
            table.Controls.Add(CreateLabel("Họ và tên:"), 0, 0);
            txtHoTen = CreateTextBox();
            table.Controls.Add(txtHoTen, 1, 0);
            
            // Chuc Vu
            table.Controls.Add(CreateLabel("Chức vụ:"), 2, 0);
            txtChucVu = CreateTextBox();
            table.Controls.Add(txtChucVu, 3, 0);

            // --- Row 1 ---
            // Bo Phan
            table.Controls.Add(CreateLabel("Bộ phận:"), 0, 1);
            cboBoPhan = CreateComboBox();
            table.Controls.Add(cboBoPhan, 1, 1);

            // Luong
            table.Controls.Add(CreateLabel("Lương CB:"), 2, 1);
            txtLuongCoBan = CreateTextBox();
            table.Controls.Add(txtLuongCoBan, 3, 1);
            
            // --- Row 2 (Extra?) ---
            // Trang Thai
            // We can put Trang Thai next to something else? 
            // Let's add explicit Row 2 for Trang Thai and empty slot
            table.Controls.Add(CreateLabel("Trạng thái:"), 0, 2);
            cboTrangThai = CreateComboBox();
            cboTrangThai.Items.AddRange(new object[] { "Đang làm", "Nghỉ việc" });
            cboTrangThai.SelectedIndex = 0;
            table.Controls.Add(cboTrangThai, 1, 2);
            
            // Add table to pnlInput
            pnlInput.Controls.Add(table);

            // Action Buttons Panel (Below Table)
            var flowActions = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom, // Changed to put at bottom of pnlInput
                FlowDirection = FlowDirection.LeftToRight,
                AutoSize = true,
                Padding = new Padding(0),
                Height = 60
            };
            // Note: Dock=Bottom in a container with AutoSize=true requires care.
            // Better to add flowActions to pnlInput *after* table, and use Dock=Top for table, Dock=Top for flow.
            
            // Re-ordering logic:
            // pnlInput.Controls.Add(table); // Top
            // pnlInput.Controls.Add(flowActions); // Below table
            
            btnThem = CreateButton("➕ Thêm", AppColors.Green);
            btnThem.Click += BtnThem_Click;

            btnSua = CreateButton("✏️ Sửa", AppColors.Blue);
            btnSua.Click += BtnSua_Click;

            btnXoa = CreateButton("🗑️ Xoá", AppColors.Red);
            btnXoa.Click += BtnXoa_Click;

            btnLamMoi = CreateButton("🔄 Làm mới", AppColors.Yellow);
            btnLamMoi.Click += (s, e) => { LamMoi(); LoadData(); };

            var lblSearch = CreateLabel("🔍 Tìm kiếm:");
            lblSearch.Margin = new Padding(30, 3, 5, 0); // Vertical center with buttons (40px)

            txtTimKiem = CreateTextBox();
            txtTimKiem.Width = 200;
            txtTimKiem.Margin = new Padding(0, 6, 0, 0); // Vertical center with buttons
            txtTimKiem.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) TimKiem(); };

            flowActions.Controls.AddRange(new Control[] { 
                btnThem, btnSua, btnXoa, btnLamMoi, 
                lblSearch, txtTimKiem 
            });

            // Add Flow AFTER Table, both Dock=Top to stack
            table.Dock = DockStyle.Top;
            flowActions.Dock = DockStyle.Top;
            
            // Correct order: Add Flow first, then Table? No.
            // Controls.Add adds to index 0 (top of Z-order).
            // Dock=Top: The last added control is at the bottom edge of the top stack.
            // Wait. "The last control added... is docked to the edge...". 
            // Correct: The control with index 0 is docked closest to the edge.
            // So: Add Table FIRST. Add Flow SECOND.
            // Table (Index 1) -> docked top.
            // Flow (Index 0) -> docked top (pushes Table down).
            // Result: Flow is Top, Table is below. WRONG.
            // We want Table Top, Flow Below.
            // So Add Flow FIRST. Add Table SECOND.
            
            pnlInput.Controls.Clear();
            pnlInput.Controls.Add(flowActions); // Index 0 -> Top
            pnlInput.Controls.Add(table);       // Index 0 -> Top (pushes Flow down)
            // Result: Table Top, Flow Below. Correct.
            
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
                Font = AppFonts.Small,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = AppColors.Surface0,
                    ForeColor = AppColors.Text,
                    SelectionBackColor = AppColors.Blue,
                    SelectionForeColor = AppColors.Base,
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

            // Define Columns Manually
            dgv.Columns.AddRange(new DataGridViewColumn[]
            {
                new DataGridViewTextBoxColumn 
                { 
                    Name = "MaNV", 
                    DataPropertyName = "MaNV", 
                    HeaderText = "Mã NV", 
                    Width = 70,
                    DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleCenter }
                },
                new DataGridViewTextBoxColumn 
                { 
                    Name = "HoTen", 
                    DataPropertyName = "HoTen", 
                    HeaderText = "Họ và Tên", 
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                    MinimumWidth = 200,
                    DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleLeft }
                },
                new DataGridViewTextBoxColumn 
                { 
                    Name = "ChucVu", 
                    DataPropertyName = "ChucVu", 
                    HeaderText = "Chức Vụ", 
                    Width = 120,
                    DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleLeft }
                },
                new DataGridViewTextBoxColumn 
                { 
                    Name = "MaBoPhan", 
                    DataPropertyName = "MaBoPhan", 
                    HeaderText = "Mã Bộ Phận", 
                    Visible = false // Keep data but hide column
                },
                new DataGridViewTextBoxColumn 
                { 
                    Name = "TenBoPhan", 
                    DataPropertyName = "TenBoPhan", 
                    HeaderText = "Bộ Phận", 
                    Width = 120,
                    DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleLeft }
                },
                new DataGridViewTextBoxColumn 
                { 
                    Name = "LuongCoBan", 
                    DataPropertyName = "LuongCoBan", 
                    HeaderText = "Lương Cơ Bản", 
                    Width = 135,
                    DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight, Format = "N0" }
                },
                new DataGridViewTextBoxColumn 
                { 
                    Name = "TrangThai", 
                    DataPropertyName = "TrangThai", 
                    HeaderText = "Trạng Thái", 
                    Width = 110,
                    DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
                }
            });

            // Using Programmatic SortMode ensures text is TRULY centered (no glyph space reserved)
            foreach (DataGridViewColumn col in dgv.Columns)
            {
                col.SortMode = DataGridViewColumnSortMode.Programmatic;
            }

            GridHelper.FixAlignment(dgv); // Use Helper for Mono/Linux alignment fix
            
            dgv.ColumnHeaderMouseClick += Dgv_Sort;
            dgv.CellClick += Dgv_CellClick;

            pnlGrid.Controls.Add(dgv);
            mainLayout.Controls.Add(pnlGrid, 0, 1);
            
            // Order: Grid Fill, Input Top. 
            // We added Input first. It docks Top. Grid docks Fill.
            // Correct.
        }

        // --- Helpers ---
        private Label CreateLabel(string text)
        {
            return new Label
            {
                Text = text,
                Font = AppFonts.Small,
                ForeColor = AppColors.SubText,
                AutoSize = true,
                Anchor = AnchorStyles.Left | AnchorStyles.Right, // Vert center?
                TextAlign = ContentAlignment.MiddleLeft,
                Margin = new Padding(0, 8, 10, 8) // Spacing
            };
        }

        private TextBox CreateTextBox()
        {
            return new TextBox
            {
                Font = AppFonts.Body,
                BackColor = AppColors.InputBg,
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 5, 20, 5)
            };
        }

        private ComboBox CreateComboBox()
        {
            return new ComboBox
            {
                Font = AppFonts.Body,
                BackColor = AppColors.InputBg,
                ForeColor = Color.White,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 5, 20, 5),
                FlatStyle = FlatStyle.Flat
            };
        }

        private Button CreateButton(string text, Color bg)
        {
            var btn = new Button
            {
                Text = text,
                Font = AppFonts.TinyBold,
                Size = new Size(110, 36),
                BackColor = bg,
                ForeColor = AppColors.Base, // Dark text on bright button
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Margin = new Padding(0, 0, 10, 0)
            };
            btn.FlatAppearance.BorderSize = 0;
            return btn;
        }

        // ... (Keep existing Logic methods: LoadData, Events, etc.) ...
        
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
            if (string.IsNullOrEmpty(propertyName)) return;

            var list = (System.Collections.Generic.List<QuanLyNhanVien.Models.NhanVien>)dgv.DataSource;
            if (list == null) return;

            // Toggle Sort direction
            _sortAscending = !_sortAscending;
            
            var prop = typeof(QuanLyNhanVien.Models.NhanVien).GetProperty(propertyName);
            if (prop == null) return;

            if (_sortAscending)
                list.Sort((a, b) => System.Collections.Comparer.Default.Compare(prop.GetValue(a), prop.GetValue(b)));
            else
                list.Sort((a, b) => System.Collections.Comparer.Default.Compare(prop.GetValue(b), prop.GetValue(a)));

            dgv.DataSource = null;
            dgv.DataSource = list;
        }

        private bool _sortAscending = false;

        private void Dgv_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var row = dgv.Rows[e.RowIndex];
            _selectedId = (int)row.Cells["MaNV"].Value;
            txtHoTen.Text = row.Cells["HoTen"].Value?.ToString();
            txtChucVu.Text = row.Cells["ChucVu"].Value?.ToString();
            cboBoPhan.SelectedValue = row.Cells["MaBoPhan"].Value;
            txtLuongCoBan.Text = ((decimal)row.Cells["LuongCoBan"].Value).ToString("0");
            cboTrangThai.SelectedItem = row.Cells["TrangThai"].Value?.ToString();
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
                TrangThai = cboTrangThai.SelectedItem?.ToString() ?? "Đang làm"
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
                    MessageBox.Show("✅ " + result.Message, "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LamMoi();
                    LoadData();
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
                var nv = LayThongTuInput();
                var result = _service.CapNhatNhanVien(nv);

                if (result.Success)
                {
                    MessageBox.Show("✅ " + result.Message, "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LamMoi();
                    LoadData();
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
                MessageBox.Show("Vui lòng chọn nhân viên cần xoá!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var confirm = MessageBox.Show("Bạn có chắc muốn xoá nhân viên này?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes) return;

            try
            {
                var result = _service.XoaNhanVien(_selectedId);
                if (result.Success)
                {
                    MessageBox.Show("✅ " + result.Message, "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LamMoi();
                    LoadData();
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
            if (cboBoPhan.Items.Count > 0) cboBoPhan.SelectedIndex = 0;
            txtTimKiem.Clear();
        }
    }
}
