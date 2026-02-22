namespace QuanLyNhanVien.Forms
{
    partial class FormBangLuong
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.fullLayout = new System.Windows.Forms.TableLayoutPanel();
            this.pnlInput = new System.Windows.Forms.Panel();
            this.flowActions = new System.Windows.Forms.FlowLayoutPanel();
            this.btnTinhLuong = new System.Windows.Forms.Button();
            this.btnLuu = new System.Windows.Forms.Button();
            this.btnXemDS = new System.Windows.Forms.Button();
            this.btnXuatExcel = new System.Windows.Forms.Button();
            this.btnXuatTatCa = new System.Windows.Forms.Button();
            this.grpResult = new System.Windows.Forms.Panel();
            this.tableRes = new System.Windows.Forms.TableLayoutPanel();
            this.lblResHeader = new System.Windows.Forms.Label();
            this.lblResLuongTheoCong = new System.Windows.Forms.Label();
            this.lblResBHXH = new System.Windows.Forms.Label();
            this.lblResThue = new System.Windows.Forms.Label();
            this.lblResTongThucNhan = new System.Windows.Forms.Label();
            this.lblResLuongTheoCongVal = new System.Windows.Forms.Label();
            this.lblResBHXHVal = new System.Windows.Forms.Label();
            this.lblResThueVal = new System.Windows.Forms.Label();
            this.lblResTongThucNhanVal = new System.Windows.Forms.Label();
            this.row2 = new System.Windows.Forms.TableLayoutPanel();
            this.lblLuongCBLabel = new System.Windows.Forms.Label();
            this.lblLuongCoBan = new System.Windows.Forms.Label();
            this.lblNgayCong = new System.Windows.Forms.Label();
            this.txtNgayCong = new System.Windows.Forms.TextBox();
            this.lblTienUng = new System.Windows.Forms.Label();
            this.txtTienUng = new System.Windows.Forms.TextBox();
            this.row1 = new System.Windows.Forms.TableLayoutPanel();
            this.lblNhanVien = new System.Windows.Forms.Label();
            this.cboNhanVien = new System.Windows.Forms.ComboBox();
            this.lblThang = new System.Windows.Forms.Label();
            this.cboThang = new System.Windows.Forms.ComboBox();
            this.lblNam = new System.Windows.Forms.Label();
            this.cboNam = new System.Windows.Forms.ComboBox();
            this.pnlGrid = new System.Windows.Forms.Panel();
            this.dgv = new System.Windows.Forms.DataGridView();
            this.colMaNV = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colHoTen = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTenBoPhan = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colLuongCoBan = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colNgayCongThucTe = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colLuongTheoCong = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTienUng = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colBHXH = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colThue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTongThucNhan = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colMaBangLuong = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colThang = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colNam = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.fullLayout.SuspendLayout();
            this.pnlInput.SuspendLayout();
            this.flowActions.SuspendLayout();
            this.grpResult.SuspendLayout();
            this.tableRes.SuspendLayout();
            this.row2.SuspendLayout();
            this.row1.SuspendLayout();
            this.pnlGrid.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).BeginInit();
            this.SuspendLayout();
            // 
            // fullLayout
            // 
            this.fullLayout.BackColor = System.Drawing.Color.Transparent;
            this.fullLayout.ColumnCount = 1;
            this.fullLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.fullLayout.Controls.Add(this.pnlInput, 0, 0);
            this.fullLayout.Controls.Add(this.pnlGrid, 0, 1);
            this.fullLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fullLayout.Location = new System.Drawing.Point(0, 0);
            this.fullLayout.Name = "fullLayout";
            this.fullLayout.RowCount = 2;
            this.fullLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.fullLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.fullLayout.Size = new System.Drawing.Size(1000, 700);
            this.fullLayout.TabIndex = 0;
            // 
            // pnlInput
            // 
            this.pnlInput.AutoSize = true;
            this.pnlInput.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(49)))), ((int)(((byte)(50)))), ((int)(((byte)(68)))));
            this.pnlInput.Controls.Add(this.flowActions);
            this.pnlInput.Controls.Add(this.grpResult);
            this.pnlInput.Controls.Add(this.row2);
            this.pnlInput.Controls.Add(this.row1);
            this.pnlInput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlInput.Location = new System.Drawing.Point(0, 0);
            this.pnlInput.Name = "pnlInput";
            this.pnlInput.Padding = new System.Windows.Forms.Padding(20, 10, 20, 10);
            this.pnlInput.Size = new System.Drawing.Size(1000, 250);
            this.pnlInput.TabIndex = 0;
            // 
            // row1 — Selection Row
            // 
            this.row1.AutoSize = true;
            this.row1.ColumnCount = 6;
            this.row1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
            this.row1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.row1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
            this.row1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.row1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
            this.row1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.row1.Controls.Add(this.lblNhanVien, 0, 0);
            this.row1.Controls.Add(this.cboNhanVien, 1, 0);
            this.row1.Controls.Add(this.lblThang, 2, 0);
            this.row1.Controls.Add(this.cboThang, 3, 0);
            this.row1.Controls.Add(this.lblNam, 4, 0);
            this.row1.Controls.Add(this.cboNam, 5, 0);
            this.row1.Dock = System.Windows.Forms.DockStyle.Top;
            this.row1.Name = "row1";
            this.row1.Padding = new System.Windows.Forms.Padding(0, 0, 0, 10);
            this.row1.RowCount = 1;
            this.row1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.row1.Size = new System.Drawing.Size(960, 40);
            this.row1.TabIndex = 0;
            // 
            // lblNhanVien
            // 
            this.lblNhanVien.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblNhanVien.AutoSize = true;
            this.lblNhanVien.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblNhanVien.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(166)))), ((int)(((byte)(173)))), ((int)(((byte)(200)))));
            this.lblNhanVien.Margin = new System.Windows.Forms.Padding(0, 8, 10, 8);
            this.lblNhanVien.Name = "lblNhanVien";
            this.lblNhanVien.Size = new System.Drawing.Size(76, 19);
            this.lblNhanVien.TabIndex = 0;
            this.lblNhanVien.Text = "Nhân viên:";
            this.lblNhanVien.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cboNhanVien
            // 
            this.cboNhanVien.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(55)))), ((int)(((byte)(78)))));
            this.cboNhanVien.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cboNhanVien.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboNhanVien.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cboNhanVien.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.cboNhanVien.ForeColor = System.Drawing.Color.White;
            this.cboNhanVien.Margin = new System.Windows.Forms.Padding(0, 5, 20, 5);
            this.cboNhanVien.Name = "cboNhanVien";
            this.cboNhanVien.Size = new System.Drawing.Size(340, 28);
            this.cboNhanVien.TabIndex = 1;
            // 
            // lblThang
            // 
            this.lblThang.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblThang.AutoSize = true;
            this.lblThang.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblThang.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(166)))), ((int)(((byte)(173)))), ((int)(((byte)(200)))));
            this.lblThang.Margin = new System.Windows.Forms.Padding(0, 8, 10, 8);
            this.lblThang.Name = "lblThang";
            this.lblThang.Size = new System.Drawing.Size(50, 19);
            this.lblThang.TabIndex = 2;
            this.lblThang.Text = "Tháng:";
            this.lblThang.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cboThang
            // 
            this.cboThang.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(55)))), ((int)(((byte)(78)))));
            this.cboThang.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cboThang.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboThang.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cboThang.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.cboThang.ForeColor = System.Drawing.Color.White;
            this.cboThang.Margin = new System.Windows.Forms.Padding(0, 5, 20, 5);
            this.cboThang.Name = "cboThang";
            this.cboThang.Size = new System.Drawing.Size(100, 28);
            this.cboThang.TabIndex = 3;
            // 
            // lblNam
            // 
            this.lblNam.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblNam.AutoSize = true;
            this.lblNam.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblNam.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(166)))), ((int)(((byte)(173)))), ((int)(((byte)(200)))));
            this.lblNam.Margin = new System.Windows.Forms.Padding(0, 8, 10, 8);
            this.lblNam.Name = "lblNam";
            this.lblNam.Size = new System.Drawing.Size(39, 19);
            this.lblNam.TabIndex = 4;
            this.lblNam.Text = "Năm:";
            this.lblNam.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cboNam
            // 
            this.cboNam.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(55)))), ((int)(((byte)(78)))));
            this.cboNam.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cboNam.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboNam.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cboNam.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.cboNam.ForeColor = System.Drawing.Color.White;
            this.cboNam.Margin = new System.Windows.Forms.Padding(0, 5, 20, 5);
            this.cboNam.Name = "cboNam";
            this.cboNam.Size = new System.Drawing.Size(100, 28);
            this.cboNam.TabIndex = 5;
            // 
            // row2 — Input Row
            // 
            this.row2.AutoSize = true;
            this.row2.ColumnCount = 6;
            this.row2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
            this.row2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33F));
            this.row2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
            this.row2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33F));
            this.row2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
            this.row2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33F));
            this.row2.Controls.Add(this.lblLuongCBLabel, 0, 0);
            this.row2.Controls.Add(this.lblLuongCoBan, 1, 0);
            this.row2.Controls.Add(this.lblNgayCong, 2, 0);
            this.row2.Controls.Add(this.txtNgayCong, 3, 0);
            this.row2.Controls.Add(this.lblTienUng, 4, 0);
            this.row2.Controls.Add(this.txtTienUng, 5, 0);
            this.row2.Dock = System.Windows.Forms.DockStyle.Top;
            this.row2.Name = "row2";
            this.row2.Padding = new System.Windows.Forms.Padding(0, 0, 0, 10);
            this.row2.RowCount = 1;
            this.row2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.row2.Size = new System.Drawing.Size(960, 40);
            this.row2.TabIndex = 1;
            // 
            // lblLuongCBLabel
            // 
            this.lblLuongCBLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblLuongCBLabel.AutoSize = true;
            this.lblLuongCBLabel.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblLuongCBLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(166)))), ((int)(((byte)(173)))), ((int)(((byte)(200)))));
            this.lblLuongCBLabel.Margin = new System.Windows.Forms.Padding(0, 8, 10, 8);
            this.lblLuongCBLabel.Name = "lblLuongCBLabel";
            this.lblLuongCBLabel.Size = new System.Drawing.Size(72, 19);
            this.lblLuongCBLabel.TabIndex = 0;
            this.lblLuongCBLabel.Text = "Lương CB:";
            this.lblLuongCBLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblLuongCoBan
            // 
            this.lblLuongCoBan.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblLuongCoBan.AutoSize = true;
            this.lblLuongCoBan.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.lblLuongCoBan.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(226)))), ((int)(((byte)(175)))));
            this.lblLuongCoBan.Name = "lblLuongCoBan";
            this.lblLuongCoBan.Size = new System.Drawing.Size(35, 20);
            this.lblLuongCoBan.TabIndex = 1;
            this.lblLuongCoBan.Text = "0 ₫";
            this.lblLuongCoBan.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblNgayCong
            // 
            this.lblNgayCong.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblNgayCong.AutoSize = true;
            this.lblNgayCong.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblNgayCong.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(166)))), ((int)(((byte)(173)))), ((int)(((byte)(200)))));
            this.lblNgayCong.Margin = new System.Windows.Forms.Padding(0, 8, 10, 8);
            this.lblNgayCong.Name = "lblNgayCong";
            this.lblNgayCong.Size = new System.Drawing.Size(80, 19);
            this.lblNgayCong.TabIndex = 2;
            this.lblNgayCong.Text = "Ngày công:";
            this.lblNgayCong.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtNgayCong
            // 
            this.txtNgayCong.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(55)))), ((int)(((byte)(78)))));
            this.txtNgayCong.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtNgayCong.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtNgayCong.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.txtNgayCong.ForeColor = System.Drawing.Color.White;
            this.txtNgayCong.Margin = new System.Windows.Forms.Padding(0, 5, 20, 5);
            this.txtNgayCong.Name = "txtNgayCong";
            this.txtNgayCong.Size = new System.Drawing.Size(200, 27);
            this.txtNgayCong.TabIndex = 3;
            this.txtNgayCong.Text = "26";
            // 
            // lblTienUng
            // 
            this.lblTienUng.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblTienUng.AutoSize = true;
            this.lblTienUng.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblTienUng.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(166)))), ((int)(((byte)(173)))), ((int)(((byte)(200)))));
            this.lblTienUng.Margin = new System.Windows.Forms.Padding(0, 8, 10, 8);
            this.lblTienUng.Name = "lblTienUng";
            this.lblTienUng.Size = new System.Drawing.Size(68, 19);
            this.lblTienUng.TabIndex = 4;
            this.lblTienUng.Text = "Tiền ứng:";
            this.lblTienUng.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtTienUng
            // 
            this.txtTienUng.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(55)))), ((int)(((byte)(78)))));
            this.txtTienUng.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtTienUng.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtTienUng.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.txtTienUng.ForeColor = System.Drawing.Color.White;
            this.txtTienUng.Margin = new System.Windows.Forms.Padding(0, 5, 20, 5);
            this.txtTienUng.Name = "txtTienUng";
            this.txtTienUng.Size = new System.Drawing.Size(200, 27);
            this.txtTienUng.TabIndex = 5;
            this.txtTienUng.Text = "0";
            // 
            // grpResult
            // 
            this.grpResult.AutoSize = true;
            this.grpResult.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(24)))), ((int)(((byte)(37)))));
            this.grpResult.Controls.Add(this.tableRes);
            this.grpResult.Controls.Add(this.lblResHeader);
            this.grpResult.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpResult.Name = "grpResult";
            this.grpResult.Padding = new System.Windows.Forms.Padding(15);
            this.grpResult.Size = new System.Drawing.Size(960, 80);
            this.grpResult.TabIndex = 2;
            // 
            // lblResHeader
            // 
            this.lblResHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblResHeader.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblResHeader.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(190)))), ((int)(((byte)(254)))));
            this.lblResHeader.Height = 25;
            this.lblResHeader.Name = "lblResHeader";
            this.lblResHeader.Size = new System.Drawing.Size(930, 25);
            this.lblResHeader.TabIndex = 0;
            this.lblResHeader.Text = "KẾT QUẢ TÍNH LƯƠNG";
            // 
            // tableRes
            // 
            this.tableRes.AutoSize = true;
            this.tableRes.ColumnCount = 4;
            this.tableRes.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tableRes.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableRes.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tableRes.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableRes.Controls.Add(this.lblResLuongTheoCong, 0, 0);
            this.tableRes.Controls.Add(this.lblResLuongTheoCongVal, 1, 0);
            this.tableRes.Controls.Add(this.lblResBHXH, 2, 0);
            this.tableRes.Controls.Add(this.lblResBHXHVal, 3, 0);
            this.tableRes.Controls.Add(this.lblResThue, 0, 1);
            this.tableRes.Controls.Add(this.lblResThueVal, 1, 1);
            this.tableRes.Controls.Add(this.lblResTongThucNhan, 2, 1);
            this.tableRes.Controls.Add(this.lblResTongThucNhanVal, 3, 1);
            this.tableRes.Dock = System.Windows.Forms.DockStyle.Top;
            this.tableRes.Name = "tableRes";
            this.tableRes.RowCount = 2;
            this.tableRes.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tableRes.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tableRes.Size = new System.Drawing.Size(930, 50);
            this.tableRes.TabIndex = 1;
            // 
            // lblResLuongTheoCong
            // 
            this.lblResLuongTheoCong.AutoSize = true;
            this.lblResLuongTheoCong.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblResLuongTheoCong.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(166)))), ((int)(((byte)(173)))), ((int)(((byte)(200)))));
            this.lblResLuongTheoCong.Name = "lblResLuongTheoCong";
            this.lblResLuongTheoCong.Text = "Lương theo công:";
            // 
            // lblResLuongTheoCongVal
            // 
            this.lblResLuongTheoCongVal.AutoSize = true;
            this.lblResLuongTheoCongVal.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblResLuongTheoCongVal.ForeColor = System.Drawing.Color.White;
            this.lblResLuongTheoCongVal.Name = "lblResLuongTheoCongVal";
            this.lblResLuongTheoCongVal.Text = "0 ₫";
            // 
            // lblResBHXH
            // 
            this.lblResBHXH.AutoSize = true;
            this.lblResBHXH.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblResBHXH.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(166)))), ((int)(((byte)(173)))), ((int)(((byte)(200)))));
            this.lblResBHXH.Name = "lblResBHXH";
            this.lblResBHXH.Text = "BHXH (10.5%):";
            // 
            // lblResBHXHVal
            // 
            this.lblResBHXHVal.AutoSize = true;
            this.lblResBHXHVal.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblResBHXHVal.ForeColor = System.Drawing.Color.White;
            this.lblResBHXHVal.Name = "lblResBHXHVal";
            this.lblResBHXHVal.Text = "0 ₫";
            // 
            // lblResThue
            // 
            this.lblResThue.AutoSize = true;
            this.lblResThue.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblResThue.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(166)))), ((int)(((byte)(173)))), ((int)(((byte)(200)))));
            this.lblResThue.Name = "lblResThue";
            this.lblResThue.Text = "Thuế TNCN:";
            // 
            // lblResThueVal
            // 
            this.lblResThueVal.AutoSize = true;
            this.lblResThueVal.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblResThueVal.ForeColor = System.Drawing.Color.White;
            this.lblResThueVal.Name = "lblResThueVal";
            this.lblResThueVal.Text = "0 ₫";
            // 
            // lblResTongThucNhan
            // 
            this.lblResTongThucNhan.AutoSize = true;
            this.lblResTongThucNhan.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblResTongThucNhan.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(166)))), ((int)(((byte)(173)))), ((int)(((byte)(200)))));
            this.lblResTongThucNhan.Name = "lblResTongThucNhan";
            this.lblResTongThucNhan.Text = "THỰC NHẬN:";
            // 
            // lblResTongThucNhanVal
            // 
            this.lblResTongThucNhanVal.AutoSize = true;
            this.lblResTongThucNhanVal.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblResTongThucNhanVal.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(166)))), ((int)(((byte)(227)))), ((int)(((byte)(161)))));
            this.lblResTongThucNhanVal.Name = "lblResTongThucNhanVal";
            this.lblResTongThucNhanVal.Text = "0 ₫";
            // 
            // flowActions
            // 
            this.flowActions.AutoSize = true;
            this.flowActions.Controls.Add(this.btnTinhLuong);
            this.flowActions.Controls.Add(this.btnLuu);
            this.flowActions.Controls.Add(this.btnXemDS);
            this.flowActions.Controls.Add(this.btnXuatExcel);
            this.flowActions.Controls.Add(this.btnXuatTatCa);
            this.flowActions.Dock = System.Windows.Forms.DockStyle.Top;
            this.flowActions.FlowDirection = System.Windows.Forms.FlowDirection.LeftToRight;
            this.flowActions.Name = "flowActions";
            this.flowActions.Padding = new System.Windows.Forms.Padding(0, 10, 0, 0);
            this.flowActions.Size = new System.Drawing.Size(960, 46);
            this.flowActions.TabIndex = 3;
            // 
            // btnTinhLuong
            // 
            this.btnTinhLuong.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(137)))), ((int)(((byte)(180)))), ((int)(((byte)(250)))));
            this.btnTinhLuong.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnTinhLuong.FlatAppearance.BorderSize = 0;
            this.btnTinhLuong.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTinhLuong.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnTinhLuong.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(46)))));
            this.btnTinhLuong.Margin = new System.Windows.Forms.Padding(0, 0, 10, 0);
            this.btnTinhLuong.Name = "btnTinhLuong";
            this.btnTinhLuong.Size = new System.Drawing.Size(140, 36);
            this.btnTinhLuong.TabIndex = 0;
            this.btnTinhLuong.Text = "Tính Lương";
            this.btnTinhLuong.UseVisualStyleBackColor = false;
            // 
            // btnLuu
            // 
            this.btnLuu.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(166)))), ((int)(((byte)(227)))), ((int)(((byte)(161)))));
            this.btnLuu.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnLuu.FlatAppearance.BorderSize = 0;
            this.btnLuu.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLuu.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnLuu.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(46)))));
            this.btnLuu.Margin = new System.Windows.Forms.Padding(0, 0, 10, 0);
            this.btnLuu.Name = "btnLuu";
            this.btnLuu.Size = new System.Drawing.Size(110, 36);
            this.btnLuu.TabIndex = 1;
            this.btnLuu.Text = "Lưu";
            this.btnLuu.UseVisualStyleBackColor = false;
            // 
            // btnXemDS
            // 
            this.btnXemDS.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(226)))), ((int)(((byte)(175)))));
            this.btnXemDS.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnXemDS.FlatAppearance.BorderSize = 0;
            this.btnXemDS.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnXemDS.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnXemDS.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(46)))));
            this.btnXemDS.Margin = new System.Windows.Forms.Padding(0, 0, 10, 0);
            this.btnXemDS.Name = "btnXemDS";
            this.btnXemDS.Size = new System.Drawing.Size(150, 36);
            this.btnXemDS.TabIndex = 2;
            this.btnXemDS.Text = "Xem DS Tháng";
            this.btnXemDS.UseVisualStyleBackColor = false;
            // 
            // btnXuatExcel — Xuất 1 phiếu lương đang chọn
            // 
            this.btnXuatExcel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(226)))), ((int)(((byte)(213)))));
            this.btnXuatExcel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnXuatExcel.FlatAppearance.BorderSize = 0;
            this.btnXuatExcel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnXuatExcel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnXuatExcel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(46)))));
            this.btnXuatExcel.Margin = new System.Windows.Forms.Padding(0, 0, 10, 0);
            this.btnXuatExcel.Name = "btnXuatExcel";
            this.btnXuatExcel.Size = new System.Drawing.Size(160, 36);
            this.btnXuatExcel.TabIndex = 3;
            this.btnXuatExcel.Text = "📄 Xuất 1 Phiếu";
            this.btnXuatExcel.UseVisualStyleBackColor = false;
            // 
            // btnXuatTatCa — Xuất toàn bộ phiếu lương tháng
            // 
            this.btnXuatTatCa.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(203)))), ((int)(((byte)(166)))), ((int)(((byte)(247)))));
            this.btnXuatTatCa.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnXuatTatCa.FlatAppearance.BorderSize = 0;
            this.btnXuatTatCa.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnXuatTatCa.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnXuatTatCa.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(46)))));
            this.btnXuatTatCa.Margin = new System.Windows.Forms.Padding(0, 0, 10, 0);
            this.btnXuatTatCa.Name = "btnXuatTatCa";
            this.btnXuatTatCa.Size = new System.Drawing.Size(180, 36);
            this.btnXuatTatCa.TabIndex = 4;
            this.btnXuatTatCa.Text = "📋 Xuất Tất Cả Tháng";
            this.btnXuatTatCa.UseVisualStyleBackColor = false;
            // 
            // pnlGrid
            // 
            this.pnlGrid.Controls.Add(this.dgv);
            this.pnlGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlGrid.Margin = new System.Windows.Forms.Padding(0, 10, 0, 0);
            this.pnlGrid.Name = "pnlGrid";
            this.pnlGrid.Size = new System.Drawing.Size(1000, 450);
            this.pnlGrid.TabIndex = 1;
            // 
            // dgv
            // 
            this.dgv.AllowUserToAddRows = false;
            this.dgv.AllowUserToDeleteRows = false;
            this.dgv.AutoGenerateColumns = false;
            this.dgv.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(46)))));
            this.dgv.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgv.ColumnHeadersDefaultCellStyle = new System.Windows.Forms.DataGridViewCellStyle()
            {
                BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(24)))), ((int)(((byte)(37))))),
                ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(166)))), ((int)(((byte)(227)))), ((int)(((byte)(161))))),
                Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold),
                Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter,
            };
            this.dgv.ColumnHeadersHeight = 45;
            this.dgv.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgv.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colMaNV, this.colHoTen, this.colTenBoPhan, this.colLuongCoBan,
            this.colNgayCongThucTe, this.colLuongTheoCong, this.colTienUng,
            this.colBHXH, this.colThue, this.colTongThucNhan,
            this.colMaBangLuong, this.colThang, this.colNam});
            this.dgv.DefaultCellStyle = new System.Windows.Forms.DataGridViewCellStyle()
            {
                BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(49)))), ((int)(((byte)(50)))), ((int)(((byte)(68))))),
                ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(205)))), ((int)(((byte)(214)))), ((int)(((byte)(244))))),
                SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(137)))), ((int)(((byte)(180)))), ((int)(((byte)(250))))),
                SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(46))))),
                Font = new System.Drawing.Font("Segoe UI", 10F),
            };
            this.dgv.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgv.EnableHeadersVisualStyles = false;
            this.dgv.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.dgv.Name = "dgv";
            this.dgv.ReadOnly = true;
            this.dgv.RowHeadersVisible = false;
            this.dgv.RowTemplate.Height = 35;
            this.dgv.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgv.Size = new System.Drawing.Size(1000, 450);
            this.dgv.TabIndex = 0;
            // 
            // colMaNV
            // 
            this.colMaNV.DataPropertyName = "MaNV"; this.colMaNV.HeaderText = "Mã NV"; this.colMaNV.Name = "colMaNV"; this.colMaNV.ReadOnly = true; this.colMaNV.Width = 70;
            this.colMaNV.DefaultCellStyle = new System.Windows.Forms.DataGridViewCellStyle() { Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter };
            this.colMaNV.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            // colHoTen
            this.colHoTen.DataPropertyName = "HoTen"; this.colHoTen.HeaderText = "Họ Tên"; this.colHoTen.Name = "colHoTen"; this.colHoTen.ReadOnly = true;
            this.colHoTen.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill; this.colHoTen.MinimumWidth = 150;
            this.colHoTen.DefaultCellStyle = new System.Windows.Forms.DataGridViewCellStyle() { Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft };
            this.colHoTen.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            // colTenBoPhan
            this.colTenBoPhan.DataPropertyName = "TenBoPhan"; this.colTenBoPhan.HeaderText = "Bộ Phận"; this.colTenBoPhan.Name = "colTenBoPhan"; this.colTenBoPhan.ReadOnly = true; this.colTenBoPhan.Width = 110;
            this.colTenBoPhan.DefaultCellStyle = new System.Windows.Forms.DataGridViewCellStyle() { Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft };
            this.colTenBoPhan.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            // colLuongCoBan
            this.colLuongCoBan.DataPropertyName = "LuongCoBan"; this.colLuongCoBan.HeaderText = "Lương CB"; this.colLuongCoBan.Name = "colLuongCoBan"; this.colLuongCoBan.ReadOnly = true; this.colLuongCoBan.Width = 110;
            this.colLuongCoBan.DefaultCellStyle = new System.Windows.Forms.DataGridViewCellStyle() { Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight, Format = "N0" };
            this.colLuongCoBan.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            // colNgayCongThucTe
            this.colNgayCongThucTe.DataPropertyName = "NgayCongThucTe"; this.colNgayCongThucTe.HeaderText = "Ngày Công"; this.colNgayCongThucTe.Name = "colNgayCongThucTe"; this.colNgayCongThucTe.ReadOnly = true; this.colNgayCongThucTe.Width = 95;
            this.colNgayCongThucTe.DefaultCellStyle = new System.Windows.Forms.DataGridViewCellStyle() { Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter };
            this.colNgayCongThucTe.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            // colLuongTheoCong
            this.colLuongTheoCong.DataPropertyName = "LuongTheoCong"; this.colLuongTheoCong.HeaderText = "Lương/Công"; this.colLuongTheoCong.Name = "colLuongTheoCong"; this.colLuongTheoCong.ReadOnly = true; this.colLuongTheoCong.Width = 115;
            this.colLuongTheoCong.DefaultCellStyle = new System.Windows.Forms.DataGridViewCellStyle() { Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight, Format = "N0" };
            this.colLuongTheoCong.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            // colTienUng
            this.colTienUng.DataPropertyName = "TienUng"; this.colTienUng.HeaderText = "Tiền Ứng"; this.colTienUng.Name = "colTienUng"; this.colTienUng.ReadOnly = true; this.colTienUng.Width = 100;
            this.colTienUng.DefaultCellStyle = new System.Windows.Forms.DataGridViewCellStyle() { Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight, Format = "N0" };
            this.colTienUng.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            // colBHXH
            this.colBHXH.DataPropertyName = "BHXH"; this.colBHXH.HeaderText = "BHXH"; this.colBHXH.Name = "colBHXH"; this.colBHXH.ReadOnly = true; this.colBHXH.Width = 95;
            this.colBHXH.DefaultCellStyle = new System.Windows.Forms.DataGridViewCellStyle() { Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight, Format = "N0" };
            this.colBHXH.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            // colThue
            this.colThue.DataPropertyName = "Thue"; this.colThue.HeaderText = "Thuế"; this.colThue.Name = "colThue"; this.colThue.ReadOnly = true; this.colThue.Width = 95;
            this.colThue.DefaultCellStyle = new System.Windows.Forms.DataGridViewCellStyle() { Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight, Format = "N0" };
            this.colThue.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            // colTongThucNhan
            this.colTongThucNhan.DataPropertyName = "TongThucNhan"; this.colTongThucNhan.HeaderText = "Thực Nhận"; this.colTongThucNhan.Name = "colTongThucNhan"; this.colTongThucNhan.ReadOnly = true; this.colTongThucNhan.Width = 115;
            this.colTongThucNhan.DefaultCellStyle = new System.Windows.Forms.DataGridViewCellStyle() { Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight, Format = "N0" };
            this.colTongThucNhan.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            // Hidden columns
            this.colMaBangLuong.DataPropertyName = "MaBangLuong"; this.colMaBangLuong.Name = "colMaBangLuong"; this.colMaBangLuong.Visible = false;
            this.colThang.DataPropertyName = "Thang"; this.colThang.Name = "colThang"; this.colThang.Visible = false;
            this.colNam.DataPropertyName = "Nam"; this.colNam.Name = "colNam"; this.colNam.Visible = false;
            // 
            // FormBangLuong
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(46)))));
            this.Controls.Add(this.fullLayout);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.ClientSize = new System.Drawing.Size(1000, 700);
            this.Name = "FormBangLuong";
            this.Text = "Bảng Lương Tháng";
            this.fullLayout.ResumeLayout(false);
            this.fullLayout.PerformLayout();
            this.pnlInput.ResumeLayout(false);
            this.pnlInput.PerformLayout();
            this.flowActions.ResumeLayout(false);
            this.grpResult.ResumeLayout(false);
            this.grpResult.PerformLayout();
            this.tableRes.ResumeLayout(false);
            this.tableRes.PerformLayout();
            this.row2.ResumeLayout(false);
            this.row2.PerformLayout();
            this.row1.ResumeLayout(false);
            this.row1.PerformLayout();
            this.pnlGrid.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel fullLayout;
        private System.Windows.Forms.Panel pnlInput;
        private System.Windows.Forms.FlowLayoutPanel flowActions;
        private System.Windows.Forms.Button btnTinhLuong;
        private System.Windows.Forms.Button btnLuu;
        private System.Windows.Forms.Button btnXemDS;
        private System.Windows.Forms.Button btnXuatExcel;
        private System.Windows.Forms.Button btnXuatTatCa;
        private System.Windows.Forms.Panel grpResult;
        private System.Windows.Forms.TableLayoutPanel tableRes;
        private System.Windows.Forms.Label lblResHeader;
        private System.Windows.Forms.Label lblResLuongTheoCong;
        private System.Windows.Forms.Label lblResBHXH;
        private System.Windows.Forms.Label lblResThue;
        private System.Windows.Forms.Label lblResTongThucNhan;
        private System.Windows.Forms.Label lblResLuongTheoCongVal;
        private System.Windows.Forms.Label lblResBHXHVal;
        private System.Windows.Forms.Label lblResThueVal;
        private System.Windows.Forms.Label lblResTongThucNhanVal;
        private System.Windows.Forms.TableLayoutPanel row2;
        private System.Windows.Forms.Label lblLuongCBLabel;
        private System.Windows.Forms.Label lblLuongCoBan;
        private System.Windows.Forms.Label lblNgayCong;
        private System.Windows.Forms.TextBox txtNgayCong;
        private System.Windows.Forms.Label lblTienUng;
        private System.Windows.Forms.TextBox txtTienUng;
        private System.Windows.Forms.TableLayoutPanel row1;
        private System.Windows.Forms.Label lblNhanVien;
        private System.Windows.Forms.ComboBox cboNhanVien;
        private System.Windows.Forms.Label lblThang;
        private System.Windows.Forms.ComboBox cboThang;
        private System.Windows.Forms.Label lblNam;
        private System.Windows.Forms.ComboBox cboNam;
        private System.Windows.Forms.Panel pnlGrid;
        private System.Windows.Forms.DataGridView dgv;
        private System.Windows.Forms.DataGridViewTextBoxColumn colMaNV;
        private System.Windows.Forms.DataGridViewTextBoxColumn colHoTen;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTenBoPhan;
        private System.Windows.Forms.DataGridViewTextBoxColumn colLuongCoBan;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNgayCongThucTe;
        private System.Windows.Forms.DataGridViewTextBoxColumn colLuongTheoCong;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTienUng;
        private System.Windows.Forms.DataGridViewTextBoxColumn colBHXH;
        private System.Windows.Forms.DataGridViewTextBoxColumn colThue;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTongThucNhan;
        private System.Windows.Forms.DataGridViewTextBoxColumn colMaBangLuong;
        private System.Windows.Forms.DataGridViewTextBoxColumn colThang;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNam;
    }
}
