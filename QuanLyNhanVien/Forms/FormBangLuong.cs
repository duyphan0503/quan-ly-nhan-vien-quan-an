using System;
using System.Drawing;
using System.Windows.Forms;
using QuanLyNhanVien.Models;
using QuanLyNhanVien.Infrastructure;
using QuanLyNhanVien.Services;

namespace QuanLyNhanVien.Forms
{
    public class FormBangLuong : Form
    {
        private ComboBox cboNhanVien;
        private ComboBox cboThang;
        private ComboBox cboNam;
        private TextBox txtNgayCong;
        private TextBox txtTienUng;
        private Label lblLuongCoBan;
        private Label lblLuongTheoCong;
        private Label lblBHXH;
        private Label lblThue;
        private Label lblTongThucNhan;
        private Button btnTinhLuong;
        private Button btnLuu;
        private Button btnXemDS;
        private DataGridView dgv;

        private readonly BangLuongService _service = new BangLuongService();
        private decimal _luongCoBan = 0;

        public FormBangLuong()
        {
            InitializeComponent();
            LoadNhanVien();
            cboThang.SelectedIndex = DateTime.Now.Month - 1;
            cboNam.SelectedItem = DateTime.Now.Year;
        }

        private void InitializeComponent()
        {
            this.Text = "Bảng Lương Tháng";
            this.BackColor = AppColors.Base;
            this.Padding = new Padding(0);

            // === INPUT PANEL ===
            var pnlInput = new Panel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                BackColor = AppColors.Surface0,
                Padding = new Padding(20, 10, 20, 10)
            };

            // Main Layout
            var mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                ColumnCount = 1,
                RowCount = 3,
                AutoSize = true
            };

            // 1. Selection Row (NV, Thang, Nam)
            var row1 = new TableLayoutPanel
            {
                ColumnCount = 6,
                RowCount = 1,
                Dock = DockStyle.Top,
                AutoSize = true,
                Padding = new Padding(0, 0, 0, 10)
            };
            row1.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize)); // Lbl
            row1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F)); // NV
            row1.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize)); // Lbl
            row1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F)); // Thang
            row1.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize)); // Lbl
            row1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F)); // Nam

            cboNhanVien = CreateComboBox();
            cboNhanVien.SelectedIndexChanged += CboNhanVien_Changed;
            
            cboThang = CreateComboBox();
            for (int i = 1; i <= 12; i++) cboThang.Items.Add(i);

            cboNam = CreateComboBox();
            int curYear = DateTime.Now.Year;
            for (int y = curYear - 5; y <= curYear + 1; y++) cboNam.Items.Add(y);

            row1.Controls.Add(CreateLabel("Nhân viên:"), 0, 0);
            row1.Controls.Add(cboNhanVien, 1, 0);
            row1.Controls.Add(CreateLabel("Tháng:"), 2, 0);
            row1.Controls.Add(cboThang, 3, 0);
            row1.Controls.Add(CreateLabel("Năm:"), 4, 0);
            row1.Controls.Add(cboNam, 5, 0);


            // 2. Input Row (LuongCB, NgayCong, TienUng)
            var row2 = new TableLayoutPanel
            {
                ColumnCount = 6,
                RowCount = 1,
                Dock = DockStyle.Top,
                AutoSize = true,
                Padding = new Padding(0, 0, 0, 10)
            };
            row2.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize)); 
            row2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33F)); 
            row2.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize)); 
            row2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33F)); 
            row2.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize)); 
            row2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33F));

            lblLuongCoBan = new Label
            {
                Text = "0 ₫",
                Font = AppFonts.BodyBold,
                ForeColor = AppColors.Yellow,
                AutoSize = true,
                Anchor = AnchorStyles.Left,
                TextAlign = ContentAlignment.MiddleLeft
            };

            txtNgayCong = CreateTextBox();
            txtNgayCong.Text = "26";

            txtTienUng = CreateTextBox();
            txtTienUng.Text = "0";

            row2.Controls.Add(CreateLabel("Lương CB:"), 0, 0);
            row2.Controls.Add(lblLuongCoBan, 1, 0);
            row2.Controls.Add(CreateLabel("Ngày công:"), 2, 0);
            row2.Controls.Add(txtNgayCong, 3, 0);
            row2.Controls.Add(CreateLabel("Tiền ứng:"), 4, 0);
            row2.Controls.Add(txtTienUng, 5, 0);


            // 3. Result Section
            var grpResult = new Panel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                BackColor = AppColors.Mantle,
                Padding = new Padding(15)
            };
            
            var lblResHeader = new Label
            {
                Text = "KẾT QUẢ TÍNH LƯƠNG",
                Font = AppFonts.SmallBold,
                ForeColor = AppColors.Lavender,
                Dock = DockStyle.Top,
                Height = 25
            };
            
            var tableRes = new TableLayoutPanel
            {
                ColumnCount = 4,
                RowCount = 2,
                Dock = DockStyle.Top,
                AutoSize = true
            };
            tableRes.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            tableRes.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableRes.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            tableRes.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));

            lblLuongTheoCong = CreateResultLabel();
            lblBHXH = CreateResultLabel();
            lblThue = CreateResultLabel();
            lblTongThucNhan = new Label 
            { 
               Text = "0 ₫", 
               Font = AppFonts.XLargeBold, 
               ForeColor = AppColors.Green, 
               AutoSize = true,
               Anchor = AnchorStyles.Left
            };

            tableRes.Controls.Add(CreateLabel("Lương theo công:"), 0, 0);
            tableRes.Controls.Add(lblLuongTheoCong, 1, 0);
            tableRes.Controls.Add(CreateLabel("BHXH (10.5%):"), 2, 0);
            tableRes.Controls.Add(lblBHXH, 3, 0);
            
            tableRes.Controls.Add(CreateLabel("Thuế TNCN:"), 0, 1);
            tableRes.Controls.Add(lblThue, 1, 1);
            tableRes.Controls.Add(CreateLabel("THỰC NHẬN:"), 2, 1);
            tableRes.Controls.Add(lblTongThucNhan, 3, 1);

            grpResult.Controls.Add(tableRes);
            grpResult.Controls.Add(lblResHeader); // Stack Top
            // ResHeader enters index 0 (Top). Table index 0 (Top).
            // Add Table first, then Header.
            grpResult.Controls.Clear();
            grpResult.Controls.Add(tableRes);
            grpResult.Controls.Add(lblResHeader);

            // 4. Buttons
            var flowActions = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                FlowDirection = FlowDirection.LeftToRight,
                AutoSize = true,
                Padding = new Padding(0, 10, 0, 0)
            };

            btnTinhLuong = CreateButton("🧮 Tính Lương", AppColors.Blue);
            btnTinhLuong.Width = 140;
            btnTinhLuong.Click += BtnTinhLuong_Click;

            btnLuu = CreateButton("💾 Lưu", AppColors.Green);
            btnLuu.Click += BtnLuu_Click;

            btnXemDS = CreateButton("📋 Xem DS Tháng", AppColors.Yellow);
            btnXemDS.Width = 150;
            btnXemDS.Click += BtnXemDS_Click;

            flowActions.Controls.AddRange(new Control[] { btnTinhLuong, btnLuu, btnXemDS });

            // Assemble Main Layout
            // We want: Row1, Row2, Result, Buttons.
            // Add to pnlInput in reverse order?
            // Or just add them sequentially with Dock=Top but maintain order logic.
            // Add Buttons (Bottom), then Result, then Row2, then Row1.
            
            pnlInput.Controls.Add(flowActions); // Dock Top
            pnlInput.Controls.Add(grpResult);   // Dock Top
            pnlInput.Controls.Add(row2);        // Dock Top
            pnlInput.Controls.Add(row1);        // Dock Top
            
            // === FULL LAYOUT ===
            var fullLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                BackColor = Color.Transparent
            };
            fullLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            fullLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            this.Controls.Add(fullLayout);

            pnlInput.Dock = DockStyle.Fill;
            fullLayout.Controls.Add(pnlInput, 0, 0);

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
                    Font = AppFonts.TinyBold,
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
                new DataGridViewTextBoxColumn { Name = "MaNV", DataPropertyName = "MaNV", HeaderText = "Mã NV", Width = 70, DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleCenter } },
                new DataGridViewTextBoxColumn { Name = "HoTen", DataPropertyName = "HoTen", HeaderText = "Họ Tên", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill, MinimumWidth = 150, DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleLeft } },
                new DataGridViewTextBoxColumn { Name = "TenBoPhan", DataPropertyName = "TenBoPhan", HeaderText = "Bộ Phận", Width = 110, DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleLeft } },
                new DataGridViewTextBoxColumn { Name = "LuongCoBan", DataPropertyName = "LuongCoBan", HeaderText = "Lương CB", Width = 110, DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleRight, Format = "N0" } },
                new DataGridViewTextBoxColumn { Name = "NgayCongThucTe", DataPropertyName = "NgayCongThucTe", HeaderText = "Ngày Công", Width = 95, DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleCenter } },
                new DataGridViewTextBoxColumn { Name = "LuongTheoCong", DataPropertyName = "LuongTheoCong", HeaderText = "Lương/Công", Width = 115, DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleRight, Format = "N0" } },
                new DataGridViewTextBoxColumn { Name = "TienUng", DataPropertyName = "TienUng", HeaderText = "Tiền Ứng", Width = 100, DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleRight, Format = "N0" } },
                new DataGridViewTextBoxColumn { Name = "BHXH", DataPropertyName = "BHXH", HeaderText = "BHXH", Width = 95, DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleRight, Format = "N0" } },
                new DataGridViewTextBoxColumn { Name = "Thue", DataPropertyName = "Thue", HeaderText = "Thuế", Width = 95, DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleRight, Format = "N0" } },
                new DataGridViewTextBoxColumn { Name = "TongThucNhan", DataPropertyName = "TongThucNhan", HeaderText = "Thực Nhận", Width = 115, DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleRight, Format = "N0" } },
                // Hidden Data Columns for logic
                new DataGridViewTextBoxColumn { Name = "MaBangLuong", DataPropertyName = "MaBangLuong", Visible = false },
                new DataGridViewTextBoxColumn { Name = "Thang", DataPropertyName = "Thang", Visible = false },
                new DataGridViewTextBoxColumn { Name = "Nam", DataPropertyName = "Nam", Visible = false }
            });

            foreach (DataGridViewColumn col in dgv.Columns)
            {
                col.SortMode = DataGridViewColumnSortMode.Programmatic;
            }

            GridHelper.FixAlignment(dgv);

            dgv.ColumnHeaderMouseClick += Dgv_Sort;
            dgv.DataBindingComplete += Dgv_DataBindingComplete;

            pnlGrid.Controls.Add(dgv);
            fullLayout.Controls.Add(pnlGrid, 0, 1);
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
                Anchor = AnchorStyles.Left,
                TextAlign = ContentAlignment.MiddleLeft,
                Margin = new Padding(0, 8, 10, 8)
            };
        }

        private Label CreateResultLabel()
        {
            return new Label
            {
                Text = "0 ₫",
                Font = AppFonts.SmallBold,
                ForeColor = Color.White,
                AutoSize = true,
                Anchor = AnchorStyles.Left,
                TextAlign = ContentAlignment.MiddleLeft
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
                ForeColor = AppColors.Base,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Margin = new Padding(0, 0, 10, 0)
            };
            btn.FlatAppearance.BorderSize = 0;
            return btn;
        }

        // Logic methods
        private void LoadNhanVien()
        {
            var list = _service.LayDanhSachNhanVien();
            cboNhanVien.DataSource = list;
            cboNhanVien.DisplayMember = "HoTen";
            cboNhanVien.ValueMember = "MaNV";
        }

        private void Dgv_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            // GridHelper already handles base alignments, 
            // but we can add specific logic here if needed.
        }

        private void Dgv_Sort(object sender, DataGridViewCellMouseEventArgs e)
        {
            string propertyName = dgv.Columns[e.ColumnIndex].DataPropertyName;
            if (string.IsNullOrEmpty(propertyName)) return;

            var list = (System.Collections.Generic.List<QuanLyNhanVien.Models.BangLuong>)dgv.DataSource;
            if (list == null) return;

            _sortAscending = !_sortAscending;
            var prop = typeof(QuanLyNhanVien.Models.BangLuong).GetProperty(propertyName);
            if (prop == null) return;

            if (_sortAscending)
                list.Sort((a, b) => System.Collections.Comparer.Default.Compare(prop.GetValue(a), prop.GetValue(b)));
            else
                list.Sort((a, b) => System.Collections.Comparer.Default.Compare(prop.GetValue(b), prop.GetValue(a)));

            dgv.DataSource = null;
            dgv.DataSource = list;
        }

        private bool _sortAscending = false;

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
                MessageBox.Show("Ngày công không hợp lệ!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            decimal tienUng;
            if (!decimal.TryParse(txtTienUng.Text, out tienUng))
            {
                MessageBox.Show("Tiền ứng không hợp lệ!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var result = _service.TinhLuong(_luongCoBan, ngayCong, tienUng);
            if (!result.Success)
            {
                MessageBox.Show(result.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var kq = result.Data;
            lblLuongTheoCong.Text = kq.LuongTheoCong.ToString("N0") + " ₫";
            lblBHXH.Text = kq.BHXH.ToString("N0") + " ₫";
            lblThue.Text = kq.Thue.ToString("N0") + " ₫";
            lblTongThucNhan.Text = kq.TongThucNhan.ToString("N0") + " ₫";
        }

        private void BtnLuu_Click(object sender, EventArgs e)
        {
            if (cboNhanVien.SelectedValue == null || cboThang.SelectedItem == null || cboNam.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn đầy đủ thông tin!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            decimal ngayCong;
            if (!decimal.TryParse(txtNgayCong.Text, out ngayCong))
            {
                MessageBox.Show("Ngày công không hợp lệ!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            decimal tienUng;
            if (!decimal.TryParse(txtTienUng.Text, out tienUng)) tienUng = 0;

            try
            {
                int maNV = (int)cboNhanVien.SelectedValue;
                int thang = (int)cboThang.SelectedItem;
                int nam = (int)cboNam.SelectedItem;

                var result = _service.LuuBangLuong(maNV, thang, nam, _luongCoBan, ngayCong, tienUng);

                if (result.Success)
                    MessageBox.Show("✅ " + result.Message, "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                else
                    MessageBox.Show(result.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnXemDS_Click(object sender, EventArgs e)
        {
            if (cboThang.SelectedItem == null || cboNam.SelectedItem == null) return;
            int thang = (int)cboThang.SelectedItem;
            int nam = (int)cboNam.SelectedItem;

            var list = _service.LayTheoThangNam(thang, nam);
            dgv.DataSource = null;
            dgv.DataSource = list;
        }
    }
}
